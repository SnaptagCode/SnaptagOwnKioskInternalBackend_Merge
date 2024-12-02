﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SnaptagOwnKioskInternalBackend.DBContexts;

#nullable disable

namespace SnaptagOwnKioskInternalBackend.Migrations
{
    [DbContext(typeof(SnaptagKioskDBContext))]
    [Migration("20241116190351_blah")]
    partial class blah
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("SnaptagOwnKioskInternalBackend.DBContexts.DBModels.PurchaseHistoryModel", b =>
                {
                    b.Property<int>("Index")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Amount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ApprovalNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthSeqNum")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("EventIndex")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MachineIndex")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PhotoAuthNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PurchaseDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("isPrinted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isRefunded")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("isUploaded")
                        .HasColumnType("INTEGER");

                    b.HasKey("Index");

                    b.ToTable("PurchaseHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
