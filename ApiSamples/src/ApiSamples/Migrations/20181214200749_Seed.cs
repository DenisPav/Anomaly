﻿using ApiSamples.Domain;
using Bogus;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiSamples.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var candidates = new Faker<Candidate>()
                .RuleFor(x => x.Id, () => 0)
                .RuleFor(x => x.Name, f => f.Name.FirstName())
                .RuleFor(x => x.Surname, f => f.Name.LastName());

            var generated = candidates.Generate(1000);

            generated.ForEach(candidate =>
            {
                migrationBuilder.InsertData("Candidate", columns: new[] { "Name", "Surname" }, values: new[] { candidate.Name, candidate.Surname });
            });


            var companies = new Faker<Company>()
                .RuleFor(x => x.Id, () => 0)
                .RuleFor(x => x.Name, f => f.Name.FirstName());

            var generatedCompanies = companies.Generate(1000);

            generatedCompanies.ForEach(company =>
            {
                migrationBuilder.InsertData("Company", columns: new[] { "Name" }, values: new[] { company.Name });
            });

            //var positions = new Faker<Position>()
            //    .RuleFor(x => x.Id, () => 0)
            //    .RuleFor(x => x.Name, f => f.Name.FirstName())
            //    .RuleFor(x => x.Openings, f => f.Random.Number(int.MaxValue));

            //var generatedPositions = positions.Generate(1000);

            //generatedPositions.ForEach(position =>
            //{
            //    migrationBuilder.InsertData("Candidate", columns: new[] { "Name", "Openings" }, values: new[] { position.Name, position.Openings });
            //});

            //var matches = new Faker<Match>()
            //    .RuleFor(x => x.Candidate, f => f.PickRandom(generated))
            //    .RuleFor(x => x.Position, f => f.PickRandom(generatedPositions));

            //var generatedMatches = positions.Generate(1000);

            //generatedPositions.ForEach(position =>
            //{
            //    migrationBuilder.InsertData("Candidate", columns: new[] { "Name", "Openings" }, values: new[] { position.Name, position.Openings });
            //});


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
