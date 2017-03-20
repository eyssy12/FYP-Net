using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CMSWebConfigurationData.Migrations
{
    public partial class _2016april02_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Course_Department_DepartmentId", table: "Course");
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
            migrationBuilder.DropTable("TimetableEvent");
            migrationBuilder.AddColumn<int>(
                name: "TimetableId",
                table: "Event",
                nullable: false,
                defaultValue: 0);
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
            migrationBuilder.DropColumn(name: "TimetableId", table: "Event");
            migrationBuilder.CreateTable(
                name: "TimetableEvent",
                columns: table => new
                {
                    TimetableId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetableEvent", x => new { x.TimetableId, x.EventId });
                    table.ForeignKey(
                        name: "FK_TimetableEvent_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetableEvent_Timetable_TimetableId",
                        column: x => x.TimetableId,
                        principalTable: "Timetable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.AddForeignKey(
                name: "FK_Course_Department_DepartmentId",
                table: "Course",
                column: "DepartmentId",
                principalTable: "Department",
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
