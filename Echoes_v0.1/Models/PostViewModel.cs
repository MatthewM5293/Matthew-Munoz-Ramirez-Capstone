using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models;

/// <summary>
/// 
/// </summary>
public class PostViewModel
{
    public PostModel Post { get; set; }

    public CommentModel Comment { get; set; }

    //Empty Constructor 
    public PostViewModel()
    {
    }
}