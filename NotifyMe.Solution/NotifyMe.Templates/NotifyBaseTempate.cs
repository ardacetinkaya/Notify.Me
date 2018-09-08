using System;
using System.Composition;
using NotifyMe.Common;

namespace NotifyMe.Templates
{
    [Export("Templates", typeof(IBaseTemplate))]
    public class NotifyBaseTemplate : IBaseTemplate
    {
        public string Name => "Base Notification";

        public string Create(string message, string from, string image)
        {
            var messageContainer = @"<div class='modal fade' id='centralModalInfo' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='true'>
<div class='modal-dialog modal-side modal-top-right' role='document'>
    <div class='modal-content'>
        <div class='modal-header'>
            <p class='heading lead'>{0}</p>

            <button type='button' class='close' data-dismiss='modal' aria-label='Close'>
                <span aria-hidden='true' class='white-text'>&times;</span>
            </button>
        </div>

        <div class='modal-body'>
            <div class='text-center' id='notificationcontent'>
                <i class='fa fa-check fa-4x mb-3 animated rotateIn'></i>
                <p>{1}</p>
            </div>
        </div>
        <div class='modal-footer justify-content-center'>
            {2}
            <a type='button' class='btn btn-outline-primary waves-effect' data-dismiss='modal'>Ok, thanks...</a>
        </div>
    </div>
</div>
</div>";
            return messageContainer;
        }
    }
}