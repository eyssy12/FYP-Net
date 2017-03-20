namespace CMS.Web.Configuration.Data
{
    public class Enumerations
    {
        public enum EnrollmentStage
        {
            First = 1,
            Second,
            Third,
            Fourth,
        }

        public enum DegreeAward
        {
            HigherDiploma = 1,
            BacherlorsOrdinary,
            BacherlorsHigher,
            Masters,
            PhD
        }

        public enum ModuleType
        {
            Mandatory = 1,
            Elective
        }

        public enum UserRole
        {
            Student = 1,
            Lecturer,
            Admin
        }
    }
}