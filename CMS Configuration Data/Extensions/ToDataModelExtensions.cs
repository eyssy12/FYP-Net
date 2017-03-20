namespace CMS.Configuration.Data.Extensions
{
    using System;
    using GcmMobileClientData = CMS.Configuration.Data.Models.GcmMobileClient;
    using GcmMobileClientModel = CMS.Shared.Library.Models.GcmMobileClient;
    using StudentData = CMS.Configuration.Data.Models.Student;
    using StudentModel = CMS.Shared.Library.Models.Student;

    public static class ToDataModelExtensions
    {
        public static GcmMobileClientData ToDataModel(this GcmMobileClientModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "");
            }

            return new GcmMobileClientData
            {
                Id = model.Id,
                Token = model.Token
            };
        }

        public static StudentData ToDataModel(this StudentModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "");
            }

            return new StudentData
            {
                Id = model.Id,
                EnrollmentDate = model.EnrollmentDate
                // TODO: finish
            };
        }
    }
}