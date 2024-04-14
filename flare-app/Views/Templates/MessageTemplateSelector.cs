using flare_app.Models;

namespace flare_app.Views.Templates
{
    internal class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? SenderTemplate { get; set; }
        public DataTemplate? ReceiverTemplate { get; set; }

        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            var msg = (Message)item;

            if (msg.Sender == null)
            {
                return ReceiverTemplate;
            }

            //else
            return SenderTemplate;
        }
    }
}
