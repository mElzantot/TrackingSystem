using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanoHealthSuite.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserInputsAndRawResponseColumnsToProcessExecutionAndValidationLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValidationLogs_ProcessExecutions_ProcessExecutionId",
                table: "ValidationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ValidationLogs_Processes_ProcessId",
                table: "ValidationLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "WorkflowSteps",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Workflows",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "ValidationLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProcessExecutionId",
                table: "ValidationLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "RawResponse",
                table: "ValidationLogs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StepId",
                table: "ValidationLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserInputs",
                table: "ProcessExecutions",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationLogs_StepId",
                table: "ValidationLogs",
                column: "StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_ValidationLogs_ProcessExecutions_ProcessExecutionId",
                table: "ValidationLogs",
                column: "ProcessExecutionId",
                principalTable: "ProcessExecutions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ValidationLogs_Processes_ProcessId",
                table: "ValidationLogs",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValidationLogs_WorkflowSteps_StepId",
                table: "ValidationLogs",
                column: "StepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValidationLogs_ProcessExecutions_ProcessExecutionId",
                table: "ValidationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ValidationLogs_Processes_ProcessId",
                table: "ValidationLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ValidationLogs_WorkflowSteps_StepId",
                table: "ValidationLogs");

            migrationBuilder.DropIndex(
                name: "IX_ValidationLogs_StepId",
                table: "ValidationLogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "RawResponse",
                table: "ValidationLogs");

            migrationBuilder.DropColumn(
                name: "StepId",
                table: "ValidationLogs");

            migrationBuilder.DropColumn(
                name: "UserInputs",
                table: "ProcessExecutions");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "WorkflowSteps",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessId",
                table: "ValidationLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessExecutionId",
                table: "ValidationLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ValidationLogs_ProcessExecutions_ProcessExecutionId",
                table: "ValidationLogs",
                column: "ProcessExecutionId",
                principalTable: "ProcessExecutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ValidationLogs_Processes_ProcessId",
                table: "ValidationLogs",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id");
        }
    }
}
