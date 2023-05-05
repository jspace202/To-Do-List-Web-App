using CCFinal.CanvasIntegration.PublishEvents;

namespace CCFinal.CanvasIntegration;

public static class Helpers {
    public static DateTime MaxDateTime(this DateTime dateTime, DateTime otherDateTime) {
        return dateTime >= otherDateTime
            ? dateTime
            : otherDateTime;
    }

    public static TaskTypeDTO ParseTaskType(this string[]? taskTypes) {
        if (taskTypes is null || taskTypes.Length == 0)
            return TaskTypeDTO.Task;

        // Try and parse tasks
        if (taskTypes.Any(x => x.Contains("quiz", StringComparison.CurrentCultureIgnoreCase)))
            return TaskTypeDTO.Quiz;
        if (taskTypes.Any(x => x.Contains("upload")))
            return TaskTypeDTO.Assignment;
        if (taskTypes.Any(x => x.Contains("Discussion", StringComparison.InvariantCultureIgnoreCase)))
            return TaskTypeDTO.Discussion;

        //fallback to task
        return TaskTypeDTO.Task;
    }
}