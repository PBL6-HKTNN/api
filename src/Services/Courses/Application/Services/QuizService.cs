using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Courses.Domain.Enums;
using Codemy.IdentityProto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Codemy.Courses.Application.Services
{
    internal class QuizService : IQuizService
    {
        private readonly ILogger<QuizService> _logger;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<UserAnswer> _userAnswerRepository;
        private readonly IRepository<Quiz> _quizRepository;
        private readonly IRepository<QuizAttempt> _quizAttemptRepository;
        private readonly IRepository<QuizQuestion> _quizQuestionRepository;
        private readonly IRepository<Lesson> _lessonRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IdentityService.IdentityServiceClient _client;
        private readonly IUnitOfWork _unitOfWork;
        public QuizService(
            ILogger<QuizService> logger,
            IRepository<Answer> answerRepository,
            IRepository<UserAnswer> userAnswerRepository,
            IRepository<Quiz> quizRepository,
            IRepository<QuizAttempt> quizAttemptRepository,
            IRepository<Lesson> lessonRepository,
            IRepository<QuizQuestion> quizQuestionRepository,
            IHttpContextAccessor httpContextAccessor,
            IdentityService.IdentityServiceClient client,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _answerRepository = answerRepository;
            _userAnswerRepository = userAnswerRepository;
            _quizRepository = quizRepository;
            _quizAttemptRepository = quizAttemptRepository;
            _quizQuestionRepository = quizQuestionRepository;
            _httpContextAccessor = httpContextAccessor;
            _lessonRepository = lessonRepository;
            _unitOfWork = unitOfWork;
            _client = client;
        }

        public async Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new QuizResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var lesson = await _lessonRepository.GetByIdAsync(request.LessonId);
            if (lesson == null || lesson.IsDeleted)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Lesson does not exist."
                };
            }

            var existingQuiz = _quizRepository.TableNoTracking.FirstOrDefault(q => q.lessonId == request.LessonId && !q.IsDeleted);
            if (existingQuiz != null)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "A quiz for this lesson already exists."
                };
            }
            int totalMarks = 0;

            Guid guid = Guid.NewGuid();
            Quiz quiz = new Quiz
            {
                Id = guid,
                lessonId = request.LessonId,
                title = request.Title,
                description = request.Description,
                passingMarks = request.passingMarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
            };
            foreach (var item in request.Questions)
            {
                Guid questionId = Guid.NewGuid();
                var question = new QuizQuestion
                {
                    Id = questionId,
                    quizId = guid,
                    questionText = item.QuestionText,
                    questionType = (QuestionType)item.QuestionType,
                    marks = item.Marks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                };
                await _quizQuestionRepository.AddAsync( question );
                totalMarks += item.Marks;
                var answersToAdd = item.Answers.Select(answerItem => new Answer
                {
                    Id = Guid.NewGuid(),
                    questionId = questionId,
                    answerText = answerItem.AnswerText,
                    isCorrect = answerItem.IsCorrect,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                });
                foreach (var answer in answersToAdd) 
                { 
                    await _answerRepository.AddAsync(answer);
                }
            }
            quiz.totalMarks = totalMarks;
            await _quizRepository.AddAsync(quiz); 
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Failed to create quiz"
                };
            }
            return new QuizResponse
            {
                Success = true,
                Message = "Quiz created successfully",
                Quiz = quiz
            };
        }

        public async Task<QuizResponse> DeleteQuizAsync(Guid quizId)
        { 
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Quiz not found"
                };
            }

            _quizRepository.Delete(quiz);
            var quizQuestions = _quizQuestionRepository.TableNoTracking.Where(q => q.quizId == quizId).ToList();
            foreach (var question in quizQuestions)
            {
                var answers = _answerRepository.TableNoTracking.Where(a => a.questionId == question.Id).ToList();
                foreach (var answer in answers)
                {
                    _answerRepository.Delete(answer);
                }
                _quizQuestionRepository.Delete(question);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Failed to delete quiz"
                };
            }
            return new QuizResponse
            {
                Success = true,
                Message = "Quiz deleted successfully"
            }; 
        }

        public async Task<QuizAttemptDtoResponse> GetQuizAttemptsAsync(Guid quizId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var quiz = await GetQuizByIdAsync(quizId);
            if (quiz.Quiz == null)
            {
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "Quiz not found."
                };
            }

            var existingAttempt = _quizAttemptRepository.TableNoTracking
                .FirstOrDefault(qa => qa.quizId == quizId && qa.userId == userId && qa.status == QuizAttemptStatus.InProgress);
            if (existingAttempt != null)
            {  
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "Existing quiz attempt retrieved successfully",
                    QuizAttempt = new QuizAttemptDto
                    {
                        Quiz = quiz.Quiz,
                        QuizAttempt = existingAttempt
                    }
                };
            }

            var successAttempt = _quizAttemptRepository.TableNoTracking
                .FirstOrDefault(qa => qa.quizId == quizId && qa.userId == userId && qa.status == QuizAttemptStatus.Completed);
            if (successAttempt != null)
            {
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "User has already completed this quiz",
                    QuizAttempt = new QuizAttemptDto
                    {
                        Quiz = quiz.Quiz,
                        QuizAttempt = successAttempt
                    }
                };
            }
            QuizAttempt quizAttempt = new QuizAttempt
            {
                Id = Guid.NewGuid(),
                quizId = quizId,
                userId = userId,
                attemptedAt = DateTime.UtcNow,
                score = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                status = QuizAttemptStatus.InProgress,
            };
            await _quizAttemptRepository.AddAsync(quizAttempt);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new QuizAttemptDtoResponse
                {
                    Success = false,
                    Message = "Failed to create quiz attempt"
                };
            }
            return new QuizAttemptDtoResponse
            {
                Success = true,
                Message = "Quiz attempt created successfully",
                QuizAttempt = new QuizAttemptDto
                {
                    Quiz = quiz.Quiz,
                    QuizAttempt = quizAttempt
                }
            };
        }

        public async Task<QuizDtoResponse> GetQuizByIdAsync(Guid id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null)
            {
                return new QuizDtoResponse
                {
                    Success = false,
                    Message = "Quiz not found."
                };
            }
            if(quiz.IsDeleted)
            {
                return new QuizDtoResponse
                {
                    Success = false,
                    Message = "Quiz is deleted."
                };
            }
            QuizDto quizDto = new QuizDto
            {
                Id = quiz.Id,
                LessonId = quiz.lessonId,
                Title = quiz.title,
                Description = quiz.description,
                TotalMarks = quiz.totalMarks,
                PassingMarks = quiz.passingMarks,
                Questions = new List<QuizQuestionDto>()
            };
            var questions = _quizQuestionRepository.TableNoTracking.Where(q => q.quizId == id && !q.IsDeleted).ToList();
            foreach (var question in questions)
            {
                QuizQuestionDto questionDto = new QuizQuestionDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.questionText,
                    QuestionType = (int)question.questionType,
                    Marks = question.marks,
                    Answers = new List<AnswerDto>()
                };
                var answers = _answerRepository.TableNoTracking.Where(a => a.questionId == question.Id && !a.IsDeleted).ToList();
                foreach (var answer in answers)
                {
                    AnswerDto answerDto = new AnswerDto
                    {
                        AnswerId = answer.Id,
                        AnswerText = answer.answerText,
                        IsCorrect = answer.isCorrect
                    };
                    questionDto.Answers.Add(answerDto);
                }
                quizDto.Questions.Add(questionDto);
            }
            return new QuizDtoResponse
            {
                Success = true,
                Message = "Quiz retrieved successfully.",
                Quiz = quizDto
            };
        }

        public async Task<QuizDtoResponse> GetQuizByLessonIdAsync(Guid lessonId)
        {
            var quiz = _quizRepository.TableNoTracking.FirstOrDefault(q => q.lessonId == lessonId && !q.IsDeleted);
            if (quiz == null)
            {
                return new QuizDtoResponse
                {
                    Success = false,
                    Message = "Quiz not found for the lesson."
                };
            }
            return await GetQuizByIdAsync(quiz.Id);
        }

        public async Task<QuizResult> GetQuizResultsAsync(Guid lessonId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new QuizResult
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            var quiz = _quizRepository.TableNoTracking.FirstOrDefault(q => q.lessonId == lessonId && !q.IsDeleted);
            if (quiz == null)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "Quiz not found for the lesson."
                };
            }
            QuizAttemptResult quizAttemptResult = new QuizAttemptResult();
            quizAttemptResult.QuizId = quiz.Id;
            var quizAttempt = _quizAttemptRepository.TableNoTracking
                .FirstOrDefault(qa => qa.quizId == quiz.Id && qa.userId == userId && (qa.status == QuizAttemptStatus.Completed || qa.status == QuizAttemptStatus.Failed));
            if (quizAttempt == null)
                {
                return new QuizResult
                {
                    Success = false,
                    Message = "No quiz attempt found for the user."
                };
            }
            quizAttemptResult.Score = quizAttempt.score;
            quizAttemptResult.Passed = quizAttempt.status == QuizAttemptStatus.Completed;
            var userAnswers = _userAnswerRepository.TableNoTracking
                .Where(ua => ua.attemptId == quizAttempt.Id)
                .ToList();
            quizAttemptResult.UserAnswers = userAnswers;
            return new QuizResult
            {
                Success = true,
                Message = "Quiz results retrieved successfully.",
                QuizAttempts = quizAttemptResult
            };
        }

        public async Task<QuizResult> SubmitQuizAsync(SubmitQuizRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new QuizResult
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            var quizAttempt = await _quizAttemptRepository.GetByIdAsync(request.QuizAttemptId);
            if (quizAttempt == null || quizAttempt.userId != userId)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "Quiz attempt not found or unauthorized."
                };
            }

            if (quizAttempt.status ==  QuizAttemptStatus.Completed)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "Quiz attempt already passed."
                };
            }

            int totalScore = 0;
            List<UserAnswer> userAnswers = new List<UserAnswer>();
            foreach (var answer in request.Answers)
            {
                var question = await _quizQuestionRepository.GetByIdAsync(answer.QuestionId);
                if (question == null)
                {
                    return new QuizResult
                    {
                        Success = false,
                        Message = $"Question with ID {answer.QuestionId} not found."
                    };
                } 
                var correctAnswers = await _answerRepository.FindAsync(x => x.questionId == answer.QuestionId);
                var correctList = correctAnswers.ToList();
                if (question.questionType == QuestionType.ShortAnswer)
                {
                    string correctText = correctList.FirstOrDefault()?.answerText?.Trim() ?? "";
                    if (string.Equals(answer.answerText?.Trim(), correctText, StringComparison.OrdinalIgnoreCase))
                        totalScore += question.marks;
                    var userAnswer = new UserAnswer
                    {
                        attemptId = quizAttempt.Id,
                        questionId = answer.QuestionId,
                        answerText = answer.answerText?.Trim(),
                        CreatedBy = userId,
                    };
                    userAnswers.Add(userAnswer);
                }
                else
                {
                    var correctIds = correctList.Where(x => x.isCorrect).Select(x => x.Id).ToList();
                    var selectedIds = answer.SelectedAnswerIds ?? new List<Guid>();

                    bool isCorrect = !correctIds.Except(selectedIds).Any() &&
                                     !selectedIds.Except(correctIds).Any();

                    if (isCorrect)
                        totalScore += question.marks;
                    userAnswers = selectedIds
                        .Select(id => new UserAnswer
                        {
                            answerId = id,
                            attemptId = quizAttempt.Id,
                            questionId = answer.QuestionId,
                            CreatedBy = userId,
                        })
                        .ToList();
                } 
            }
            Quiz quiz = await _quizRepository.GetByIdAsync(quizAttempt.quizId);
            if (quiz == null)
            {
                return new QuizResult
                {
                    Success = false,
                    Message = "Quiz not found."
                };
            }
            quizAttempt.score = totalScore;
            quizAttempt.completedAt = DateTime.UtcNow;
            quizAttempt.status = totalScore >= quiz.passingMarks
                ? QuizAttemptStatus.Completed
                : QuizAttemptStatus.Failed;

            _quizAttemptRepository.Update(quizAttempt); 
            foreach (var userAnswer in userAnswers)
            {
                await _userAnswerRepository.AddAsync(userAnswer);

            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError("Failed to submit for QuizAttempt ID {QuizAttemptId}.", request.QuizAttemptId);
                return new QuizResult
                {
                    Success = false,
                    Message = "Failed to submit."
                };
            }
            _logger.LogInformation("Submit successfully");
            return new QuizResult
            {
                Success = true,
                Message = "Submit successfully.",
                QuizAttempts = new QuizAttemptResult
                {
                    QuizId = quizAttempt.quizId,
                    Score = quizAttempt.score,
                    Passed = quizAttempt.status == QuizAttemptStatus.Completed,
                    UserAnswers = userAnswers
                },
            };
        }

        public async Task<QuizResponse> UpdateQuizAsync(Guid quizId, CreateQuizRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            var userExists = await _client.GetUserByIdAsync(
                new GetUserByIdRequest { UserId = userId.ToString() }
            );

            if (!userExists.Exists)
            {
                _logger.LogError("User with ID {UserId} does not exist.", userId);
                return new QuizResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }
            
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Quiz not found."
                };
            }
            if (quiz.CreatedBy != userId)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "User not authorized to update this quiz."
                };
            }
            quiz.lessonId = request.LessonId;
            quiz.title = request.Title;
            quiz.description = request.Description;
            quiz.passingMarks = request.passingMarks;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.UpdatedBy = userId;

            // Delete existing questions and answers
            var existingQuestions = _quizQuestionRepository.TableNoTracking.Where(q => q.quizId == quizId).ToList();
            foreach (var question in existingQuestions)
            {
                var answers = _answerRepository.TableNoTracking.Where(a => a.questionId == question.Id).ToList();
                foreach (var answer in answers)
                {
                    _answerRepository.Delete(answer);
                }
                _quizQuestionRepository.Delete(question);
            }
            int totalMarks = 0;
            foreach (var item in request.Questions)
            { 
                Guid questionId = Guid.NewGuid();
                var question = new QuizQuestion
                {
                    Id = questionId,
                    quizId = quizId,
                    questionText = item.QuestionText,
                    questionType = (QuestionType)item.QuestionType,
                    marks = item.Marks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                };
                totalMarks += item.Marks;
                await _quizQuestionRepository.AddAsync(question);
                var answersToAdd = item.Answers.Select(answerItem => new Answer
                {
                    Id = Guid.NewGuid(),
                    questionId = questionId,
                    answerText = answerItem.AnswerText,
                    isCorrect = answerItem.IsCorrect,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = userId,
                });
                foreach (var answer in answersToAdd)
                {
                    await _answerRepository.AddAsync(answer);
                }
            }
            quiz.totalMarks = totalMarks;
            _quizRepository.Update(quiz);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new QuizResponse
                {
                    Success = false,
                    Message = "Failed to update quiz"
                };
            }
            return new QuizResponse
            {
                Success = true,
                Message = "Quiz updated successfully",
                Quiz = quiz
            };
        }
    }
}
