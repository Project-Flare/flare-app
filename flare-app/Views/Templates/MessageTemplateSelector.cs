using flare_app.Models;

namespace flare_app.Views.Templates
{
    internal class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? SenderTemplate { get; set; }
        public DataTemplate? ReceiverTemplate { get; set; }
        public DataTemplate? InfoTemplate { get; set; }

        /// <summary>
        /// Selects template for message blob depending on 'Sender' attribute.
        /// </summary>
        protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
        {
            var msg = (Message)item;

            if (msg.Sender == "ChatViewModel")
            {
                return InfoTemplate;
            }

            if (msg.Sender != null)
            {
                return ReceiverTemplate;
            }

            return SenderTemplate;
        }
    }
}
