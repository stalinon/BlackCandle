namespace BlackCandle.Domain.Entities;

/// <summary>
///     Расписание задач (cron)
/// </summary>
public class CronSchedule
{
    /// <summary>
    ///     Cron-выражение
    /// </summary>
    public string Expression { get; set; } = "* * * * *";

    /// <summary>
    ///     Часовой пояс
    /// </summary>
    public string Timezone { get; set; } = "UTC";

    /// <summary>
    ///     Следующий запуск
    /// </summary>
    public DateTime NextRun { get; set; }
}
