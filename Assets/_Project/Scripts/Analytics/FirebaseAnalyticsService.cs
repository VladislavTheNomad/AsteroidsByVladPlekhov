using Firebase.Analytics;
using Zenject;

namespace Asteroids
{
    public class FirebaseAnalyticsService : IAnalytics
    {
        private Statistics _statistics;

        [Inject]
        public void Construct(Statistics stat)
        {
            _statistics = stat;
        }
        public void LogGameEnd()
        {
            Parameter[] parameters =
            {
                new Parameter("Bullets Fired", _statistics.BulletsFired),
                new Parameter("Lasers Fired", _statistics.LasersFired),
                new Parameter("UFO Killed", _statistics.UfosKilled),
                new Parameter("Asteroids Killed", _statistics.AsteroidsKilled),
            };

            FirebaseAnalytics.LogEvent("game_end", parameters);
        }

        public void LogGameStart()
        {
            FirebaseAnalytics.LogEvent("game_start");
        }

        public void LogLaserUsed()
        {
            FirebaseAnalytics.LogEvent("laser_used");
        }
    }
}
