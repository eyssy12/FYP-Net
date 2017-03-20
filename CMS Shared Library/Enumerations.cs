namespace CMS.Shared.Library
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

        public enum AcademicStatus // TODO: rename this 
        {
            Student = 1,
            Graduate,
            Lecturer,
            AssistantLecturer,
            SeniorLecturer,
            DepartmentalDirector
        }

        public enum SpecialStatus
        {
            None = 1,
            ClassRep,
            ViceClassRep,
            YearHead,
        }

        public enum Roles : int
        {
            CanCreate = 1,
            CanRead = 2,
            CanUpdate = 4,
            CanDelete = 8
        }
    }
}