using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Course.Domain.Enum
{
    internal enum Status
    {
        Draft,
        Published,
        Archived
    }

    internal enum QuizAttemptStatus
    {
        InProgress,
        Completed,
        Failed
    }
}
