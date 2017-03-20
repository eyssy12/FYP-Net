namespace CMS.Configuration.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Library.Extensions;
    using GcmMobileClientData = CMS.Configuration.Data.Models.GcmMobileClient;
    using GcmMobileClientModel = CMS.Shared.Library.Models.GcmMobileClient;
    using StudentData = CMS.Configuration.Data.Models.Student;
    using StudentModel = CMS.Shared.Library.Models.Student;

    public static class ToApplicationModelExtensions
    {
        public static IEnumerable<GcmMobileClientModel> ToApplicationModels(this IEnumerable<GcmMobileClientData> models)
        {
            if (models.IsEmpty())
            {
                return models.AsEmpty<GcmMobileClientData, GcmMobileClientModel>();
            }

            return models.Select(m => m.ToApplicationModel()).ToArray();
        }

        public static GcmMobileClientModel ToApplicationModel(this GcmMobileClientData model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "");
            }

            return new GcmMobileClientModel
            {
                Id = model.Id,
                Token = model.Token
            };
        }

        public static StudentModel ToApplicationModel(this StudentData model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model), "");
            }

            return new StudentModel
            {
                Id = model.Id,
                EnrollmentDate = model.EnrollmentDate
            };
        }
    }
}