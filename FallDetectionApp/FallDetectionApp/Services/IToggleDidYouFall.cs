using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FallDetectionApp.Models;

namespace FallDetectionApp.Services
{
    public interface IToggleDidYouFall
    {
        bool ToggleDidYouFallMainActivity(bool isActivated);

    }
}
