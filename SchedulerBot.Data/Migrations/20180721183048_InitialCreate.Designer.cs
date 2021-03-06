﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SchedulerBot.Data.Models;

namespace SchedulerBot.Data.Migrations
{
    [DbContext(typeof(SchedulerBotContext))]
    [Migration("20180721183048_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SchedulerBot.Data.Models.Calendar", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<decimal>("DefaultChannel")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Prefix");

                    b.Property<string>("Timezone");

                    b.HasKey("Id");

                    b.ToTable("Calendars");
                });

            modelBuilder.Entity("SchedulerBot.Data.Models.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("CalendarId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset>("EndTimestamp");

                    b.Property<string>("Name");

                    b.Property<int>("Repeat");

                    b.Property<DateTimeOffset>("StartTimestamp");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("SchedulerBot.Data.Models.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("CalendarId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<bool>("IsDenied");

                    b.Property<decimal>("TargetId")
                        .HasConversion(new ValueConverter<decimal, decimal>(v => default(decimal), v => default(decimal), new ConverterMappingHints(precision: 20, scale: 0)));

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CalendarId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("SchedulerBot.Data.Models.Event", b =>
                {
                    b.HasOne("SchedulerBot.Data.Models.Calendar")
                        .WithMany("Events")
                        .HasForeignKey("CalendarId");
                });

            modelBuilder.Entity("SchedulerBot.Data.Models.Permission", b =>
                {
                    b.HasOne("SchedulerBot.Data.Models.Calendar", "Calendar")
                        .WithMany()
                        .HasForeignKey("CalendarId");
                });
#pragma warning restore 612, 618
        }
    }
}
