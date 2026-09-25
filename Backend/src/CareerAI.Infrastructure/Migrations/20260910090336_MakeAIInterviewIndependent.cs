using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAIInterviewIndependent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Applications_ApplicationId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_InterviewEvaluations_InterviewQuestionId",
                table: "InterviewEvaluations");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "AIFeedback",
                table: "InterviewQuestions");

            migrationBuilder.DropColumn(
                name: "CandidateAnswer",
                table: "InterviewQuestions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "InterviewQuestions");

            migrationBuilder.DropColumn(
                name: "EvaluatedAt",
                table: "InterviewEvaluations");

            migrationBuilder.DropColumn(
                name: "Strengths",
                table: "InterviewEvaluations");

            migrationBuilder.DropColumn(
                name: "Weaknesses",
                table: "InterviewEvaluations");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "Interviews",
                newName: "CandidateId");

            migrationBuilder.RenameIndex(
                name: "IX_Interviews_ApplicationId",
                table: "Interviews",
                newName: "IX_Interviews_CandidateId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledAt",
                table: "Interviews",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Interviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Interviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "InterviewQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_InterviewQuestionId",
                table: "InterviewEvaluations",
                column: "InterviewQuestionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Candidates_CandidateId",
                table: "Interviews",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Candidates_CandidateId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_InterviewEvaluations_InterviewQuestionId",
                table: "InterviewEvaluations");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "InterviewQuestions");

            migrationBuilder.RenameColumn(
                name: "CandidateId",
                table: "Interviews",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Interviews_CandidateId",
                table: "Interviews",
                newName: "IX_Interviews_ApplicationId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ScheduledAt",
                table: "Interviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Interviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIFeedback",
                table: "InterviewQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CandidateAnswer",
                table: "InterviewQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "InterviewQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EvaluatedAt",
                table: "InterviewEvaluations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Strengths",
                table: "InterviewEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weaknesses",
                table: "InterviewEvaluations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewEvaluations_InterviewQuestionId",
                table: "InterviewEvaluations",
                column: "InterviewQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Applications_ApplicationId",
                table: "Interviews",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
