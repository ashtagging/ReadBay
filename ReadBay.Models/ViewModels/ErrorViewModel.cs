using System;

namespace ReadBay.Models.ViewModels
{
    //Used in error view model view
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
