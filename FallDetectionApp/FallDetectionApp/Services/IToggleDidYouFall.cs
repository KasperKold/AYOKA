using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FallDetectionApp.Models;

namespace FallDetectionApp.Services
{
    public interface IToggleDidYouFall
    {
        Task<bool> ToggleDidYouFallMainActivity();

    }
}
