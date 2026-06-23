using System.Media;
using System.IO;

namespace CyberSentinel
{
    /// <summary>
    /// Manages sound playback for chatbot responses, welcome, and quiz feedback.
    /// </summary>
    public static class SoundManager
    {
        private static SoundPlayer _player;
        private static bool _soundEnabled = true;

        /// <summary>
        /// Toggles sound on/off.
        /// </summary>
        public static bool SoundEnabled
        {
            get => _soundEnabled;
            set => _soundEnabled = value;
        }

        /// <summary>
        /// Plays a welcome sound when the application starts.
        /// </summary>
        public static void PlayWelcome()
        {
            if (!_soundEnabled) return;
            // Use embedded resource or file; here we generate a simple beep for demonstration.
            // For a real app, embed a WAV file.
            System.Media.SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Plays a sound when the chatbot gives a response.
        /// </summary>
        public static void PlayResponse()
        {
            if (!_soundEnabled) return;
            System.Media.SystemSounds.Asterisk.Play();
        }

        /// <summary>
        /// Plays a sound for correct quiz answer.
        /// </summary>
        public static void PlayCorrect()
        {
            if (!_soundEnabled) return;
            System.Media.SystemSounds.Exclamation.Play();
        }

        /// <summary>
        /// Plays a sound for incorrect quiz answer.
        /// </summary>
        public static void PlayIncorrect()
        {
            if (!_soundEnabled) return;
            System.Media.SystemSounds.Hand.Play();
        }

        /// <summary>
        /// Plays a generic notification sound.
        /// </summary>
        public static void PlayNotification()
        {
            if (!_soundEnabled) return;
            System.Media.SystemSounds.Question.Play();
        }
    }
}