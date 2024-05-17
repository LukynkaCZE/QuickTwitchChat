using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput.Events.Sources;

namespace QuickTwitchChat
{
    public sealed partial class TwitchChatWindow : Form
    {
        private IKeyboardEventSource _keyboardEventSource = null!;
        
        public TwitchChatWindow()
        {
            InitializeComponent();
            webView.Source = new Uri("https://dashboard.twitch.tv/popout/u/lukynkacze/stream-manager/chat?uuid=1ea08990b43874d4c7549914f23cdc17");
            Text = "Quick Twitch Chat";
            SetVisibleCore(false);
            WindowState = FormWindowState.Minimized;
            var runnable = new AsyncRunnable(() => StartKeyboardHook());
        }

        public void focusChat()
        {
            try
            {
                webView.CoreWebView2.ExecuteScriptAsync("document.querySelectorAll('[role=textbox]')[0].focus()");
                Console.WriteLine("Invoked document.querySelectorAll('[role=textbox]')[0].focus()");
                
            } catch(Exception ex)
            {
                //ignore
            }
        }
        
        private void TwitchChatWindow_Activated(object sender, EventArgs e)
        {
            focusChat();
        }
        
        private Task StartKeyboardHook()
        {
            using (_keyboardEventSource = WindowsInput.Capture.Global.KeyboardAsync()) {
                _keyboardEventSource.KeyEvent += Keyboard_KeyEvent;
                Console.ReadLine();
            }

            return Task.CompletedTask;
        }

        private List<string> heldKeys = [];

        private void Keyboard_KeyEvent(object sender, EventSourceEventArgs<KeyboardEvent> e)
        {
            var keyDown = e.Data.KeyDown?.Key.ToString();
            var keyUp = e.Data.KeyUp?.Key.ToString();

            if(keyDown != null && !heldKeys.Contains(keyDown)) heldKeys.Add(keyDown);
            if(keyUp != null && heldKeys.Contains(keyUp)) heldKeys.Remove(keyUp);
            
            if (heldKeys.Contains("LControl") && heldKeys.Contains("T"))
            {
                e.Next_Hook_Enabled = false;
                ExecuteInMainContext(() =>
                {
                    SetVisibleCore(true);
                    WindowHook.FocusProcess();
                    Console.WriteLine("Changed Window State to NORMAL");
                });
            }

            if (!heldKeys.Contains("Escape") || this.WindowState == FormWindowState.Minimized) return;
            this.WindowState = FormWindowState.Minimized;
            Console.WriteLine("Changed Window State to MINIMIZED");
            e.Next_Hook_Enabled = false;
            SetVisibleCore(false);
        }

        private void ExecuteInMainContext(Action action)
        {
            Invoke(new MethodInvoker(action.Invoke));
        }
    }
}