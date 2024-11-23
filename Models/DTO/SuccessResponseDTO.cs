using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace TomNam.Models.DTO
{
    public class SuccessResponseDTO
    {
        public string? Message { get; set; }
        public object Data { get; set; }
    }
}