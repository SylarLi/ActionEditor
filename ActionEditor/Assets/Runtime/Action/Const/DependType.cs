namespace Action
{
    public enum DependType
    {
        Actor,      // 单个角色依赖，Index
        Actors,     // 复数角色依赖，Range
        Point,      // 单个坐标依赖，Index
        Points,     // 复数坐标依赖，Range
    }
}
