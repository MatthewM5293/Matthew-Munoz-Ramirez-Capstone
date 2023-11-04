using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models;

/// <summary>
/// 
/// </summary>
public class PostModel
{
    /*
     * PostID DONE
     * UserID (User it belongs to) DONE
     *  User's Profile picture URL DONE
     * 
     * Media (Image or Video) 1 DONE
     * Caption or Description DONE
     * Date of Post Done
     * Date Edited Done
     * Comments Done
     */

    [Key]
    public Guid PostId { get; set; }

    [ForeignKey("UserID")]
    public Guid UserId { get; set; }

    public string? UserName { get; set; }
    public string? Name { get; set; }

    public string? ProfilePicture { get; set; }

    [MaxLength(2000)]
    public string? Caption { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? ImageUrl { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime PostDate { get; set; } = DateTime.Now; //sets Posted time to now

    [DataType(DataType.DateTime)]
    public DateTime? EditDate { get; set; } //can be null as a post will not always be edited.

    //Toggle Comments feature
    public bool CommentsEnabled { get; set; } = true;

    //Likes
    public int LikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
    public List<CommentModel>? Comments { get; set; } //Comments filtered by ID 
    public List<LikeModel>? LikedBy { get; set; }

    public double Longitude { get; set; }
    public double Latitude { get; set; }

    [NotMapped]
    public bool IsNear { get; set; }
    
    [NotMapped]
    public string? TimeAgo { get; set; }

    //Empty Constructor 
    public PostModel()
    {
    }

    //Post location check
    public static bool IsNearToUsER(double location1Lat, double location1Long, double location2Lat, double location2Long, double thresholdKilometers)
    {
        // Haversine formula to calculate distance
        const double EarthRadiusKilometers = 6371.0;
        double dLat = Math.PI / 180 * (location2Long - location1Lat);
        double dLon = Math.PI / 180 * (location2Long - location1Long);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Math.PI / 180 * location1Lat) * Math.Cos(Math.PI / 180 * location2Lat) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = EarthRadiusKilometers * c;

        return distance <= thresholdKilometers;
    }
}