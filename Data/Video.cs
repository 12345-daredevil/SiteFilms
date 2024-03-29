﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteFilms.Data
{
    public class Video
    {
        [Key]
        public uint Id { get; set; }

        [ForeignKey("AspNetUsersId")]
        public string? AspNetUsersId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [ForeignKey("CountryId")]
        public int CountryId { get; set; }
        public Country? Country { get; set; }

        [ForeignKey("GenreId")]
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public ushort TimeVideo { get; set; }
        public byte AgeRestriction { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime? MakeDate { get; set; } 
        public DateTime AddDate { get; set; } = DateTime.UtcNow.Date;
        public bool FlagCheck { get; set; } = false;
        public byte[]? Skin { get; set; }

        public Video() { }

        public Video(string name, string? description, int countryId, int genreId, ushort timeVideo, byte ageRestriction, DateTime? makeDate, string userId)
        {
            Name = name;
            Description = description;
            CountryId = countryId;
            TimeVideo = timeVideo;
            AgeRestriction = ageRestriction;
            MakeDate = makeDate;
            AddDate = DateTime.UtcNow;
            GenreId = genreId;
            AspNetUsersId = userId;
        }

        public void EditVideo(Video video)
        {
            Name = video.Name;
            Description = video.Description;
            CountryId = video.CountryId;
            TimeVideo = video.TimeVideo;
            AgeRestriction = video.AgeRestriction;
            AddDate = video.AddDate;
            GenreId = video.GenreId;
            FlagCheck = video.FlagCheck;

            if (video.MakeDate != null)
                MakeDate = video.MakeDate;
        }
    }
}
