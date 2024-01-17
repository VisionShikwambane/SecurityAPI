using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurityAPI.Migrations
{
    /// <inheritdoc />
    public partial class new4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Doctors_DoctorID",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SlotDescription",
                table: "Slots");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorID",
                table: "Slots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Slots",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlotDate",
                table: "Slots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Doctors_DoctorID",
                table: "Slots",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "DoctorID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Slots_Doctors_DoctorID",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Slots");

            migrationBuilder.DropColumn(
                name: "SlotDate",
                table: "Slots");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorID",
                table: "Slots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SlotDescription",
                table: "Slots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Slots_Doctors_DoctorID",
                table: "Slots",
                column: "DoctorID",
                principalTable: "Doctors",
                principalColumn: "DoctorID");
        }
    }
}
