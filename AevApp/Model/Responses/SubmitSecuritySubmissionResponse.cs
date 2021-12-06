using System.Collections.Generic;

namespace AevApp.Model.Responses
{
    public class SubmitSecuritySubmissionResponse
    {
        public SumitSecuritySubmission Result { get; set; }
    }

    public class SumitSecuritySubmission : SecuritySubmission
    {
        public List<SecuritySubmissionCheck> Checks { get; set; }
    }

    public class SecuritySubmissionCheck
    {

        public int Id { get; set; }

        public int SecuritySubmissionId { get; set; }

        public int SecurityCheckId { get; set; }

        public bool Pass { get; set; }

        public bool Manual { get; set; }
    }
}