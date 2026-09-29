using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSessionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "SessionCheckConstrint",
                table: "Sessions");

            migrationBuilder.DropCheckConstraint(
                name: "SessionCheckConstrint2",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TrainarId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "StarttDate",
                table: "Sessions",
                newName: "StartDate");

            migrationBuilder.AddCheckConstraint(
                name: "SessionCheckConstraint",
                table: "Sessions",
                sql: "Capacity BETWEEN 1 AND 25");

            migrationBuilder.AddCheckConstraint(
                name: "SessionCheckConstraint2",
                table: "Sessions",
                sql: "StartDate < EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "SessionCheckConstraint",
                table: "Sessions");

            migrationBuilder.DropCheckConstraint(
                name: "SessionCheckConstraint2",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Sessions",
                newName: "StarttDate");

            migrationBuilder.AddColumn<int>(
                name: "TrainarId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "SessionCheckConstrint",
                table: "Sessions",
                sql: "Capacity Between 1 And 25");

            migrationBuilder.AddCheckConstraint(
                name: "SessionCheckConstrint2",
                table: "Sessions",
                sql: "StarttDate < EndDate");
        }
    }
}
