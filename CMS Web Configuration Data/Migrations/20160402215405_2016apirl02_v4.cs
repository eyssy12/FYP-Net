using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CMSWebConfigurationData.Migrations
{
    public partial class _2016apirl02_v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Course_Department_DepartmentId", table: "Course");
            migrationBuilder.DropForeignKey(name: "FK_Event_Timetable_TimetableId", table: "Event");
            migrationBuilder.DropForeignKey(name: "FK_GcmMobileClient_Person_PersonId", table: "GcmMobileClient");
            migrationBuilder.DropForeignKey(name: "FK_LecturerPerson_Lecturer_LecturerId", table: "LecturerPerson");
            migrationBuilder.DropForeignKey(name: "FK_LecturerPerson_Person_PersonId", table: "LecturerPerson");
            migrationBuilder.DropForeignKey(name: "FK_Person_ApplicationUser_ApplicationUserId", table: "Person");
            migrationBuilder.DropForeignKey(name: "FK_Semester_Course_CourseId", table: "Semester");
            migrationBuilder.DropForeignKey(name: "FK_StudentPerson_Person_PersonId", table: "StudentPerson");
            migrationBuilder.DropForeignKey(name: "FK_StudentPerson_Student_StudentId", table: "StudentPerson");
            migrationBuilder.DropForeignKey(name: "FK_Timetable_Class_ClassId", table: "Timetable");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.AddForeignKey(
                name: "FK_Course_Department_DepartmentId",
                table: "Course",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(
                name: "FK_Event_Timetable_TimetableId",
                table: "Event",
                column: "TimetableId",
                principalTable: "Timetable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_GcmMobileClient_Person_PersonId",
                table: "GcmMobileClient",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(
                name: "FK_LecturerPerson_Lecturer_LecturerId",
                table: "LecturerPerson",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_LecturerPerson_Person_PersonId",
                table: "LecturerPerson",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Person_ApplicationUser_ApplicationUserId",
                table: "Person",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(
                name: "FK_Semester_Course_CourseId",
                table: "Semester",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerson_Person_PersonId",
                table: "StudentPerson",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerson_Student_StudentId",
                table: "StudentPerson",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Timetable_Class_ClassId",
                table: "Timetable",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Course_Department_DepartmentId", table: "Course");
            migrationBuilder.DropForeignKey(name: "FK_Event_Timetable_TimetableId", table: "Event");
            migrationBuilder.DropForeignKey(name: "FK_GcmMobileClient_Person_PersonId", table: "GcmMobileClient");
            migrationBuilder.DropForeignKey(name: "FK_LecturerPerson_Lecturer_LecturerId", table: "LecturerPerson");
            migrationBuilder.DropForeignKey(name: "FK_LecturerPerson_Person_PersonId", table: "LecturerPerson");
            migrationBuilder.DropForeignKey(name: "FK_Person_ApplicationUser_ApplicationUserId", table: "Person");
            migrationBuilder.DropForeignKey(name: "FK_Semester_Course_CourseId", table: "Semester");
            migrationBuilder.DropForeignKey(name: "FK_StudentPerson_Person_PersonId", table: "StudentPerson");
            migrationBuilder.DropForeignKey(name: "FK_StudentPerson_Student_StudentId", table: "StudentPerson");
            migrationBuilder.DropForeignKey(name: "FK_Timetable_Class_ClassId", table: "Timetable");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_ApplicationUser_UserId", table: "AspNetUserRoles");
            migrationBuilder.AddForeignKey(
                name: "FK_Course_Department_DepartmentId",
                table: "Course",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Event_Timetable_TimetableId",
                table: "Event",
                column: "TimetableId",
                principalTable: "Timetable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_GcmMobileClient_Person_PersonId",
                table: "GcmMobileClient",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_LecturerPerson_Lecturer_LecturerId",
                table: "LecturerPerson",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_LecturerPerson_Person_PersonId",
                table: "LecturerPerson",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Person_ApplicationUser_ApplicationUserId",
                table: "Person",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Semester_Course_CourseId",
                table: "Semester",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerson_Person_PersonId",
                table: "StudentPerson",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_StudentPerson_Student_StudentId",
                table: "StudentPerson",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Timetable_Class_ClassId",
                table: "Timetable",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
