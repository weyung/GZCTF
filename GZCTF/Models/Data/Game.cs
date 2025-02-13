﻿using CTFServer.Models.Request.Edit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CTFServer.Models;

public class Game
{
    [Key]
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// 比赛标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 头图哈希
    /// </summary>
    public string? PosterHash { get; set; }

    /// <summary>
    /// 比赛描述
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// 比赛详细介绍
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 队员数量限制, 0 为无上限
    /// </summary>
    public int TeamMemberCountLimit { get; set; } = 0;

    /// <summary>
    /// 队伍同时开启的容器数量限制
    /// </summary>
    public int ContainerCountLimit { get; set; } = 3;

    /// <summary>
    /// 开始时间
    /// </summary>
    [Required]
    [JsonPropertyName("start")]
    public DateTimeOffset StartTimeUTC { get; set; } = DateTimeOffset.FromUnixTimeSeconds(0);

    /// <summary>
    /// 结束时间
    /// </summary>
    [Required]
    [JsonPropertyName("end")]
    public DateTimeOffset EndTimeUTC { get; set; } = DateTimeOffset.FromUnixTimeSeconds(0);

    [NotMapped]
    [JsonIgnore]
    public bool IsActive => StartTimeUTC <= DateTimeOffset.Now && DateTimeOffset.Now <= EndTimeUTC;

    #region Db Relationship

    /// <summary>
    /// 比赛事件
    /// </summary>
    [JsonIgnore]
    public List<GameEvent> GameEvents { get; set; } = new();

    /// <summary>
    /// 比赛通知
    /// </summary>
    [JsonIgnore]
    public List<GameNotice> GameNotices { get; set; } = new();

    /// <summary>
    /// 比赛题目
    /// </summary>
    [JsonIgnore]
    public List<Challenge> Challenges { get; set; } = new();

    /// <summary>
    /// 比赛提交
    /// </summary>
    [JsonIgnore]
    public List<Submission> Submissions { get; set; } = new();

    /// <summary>
    /// 比赛队伍参赛对象
    /// </summary>
    [JsonIgnore]
    public HashSet<Participation> Participations { get; set; } = new();

    /// <summary>
    /// 比赛队伍
    /// </summary>
    [JsonIgnore]
    public ICollection<Team>? Teams { get; set; }

    #endregion Db Relationship

    [NotMapped]
    public string? PosterUrl => PosterHash is null ? null : $"/assets/{PosterHash}/poster";

    public Game Update(GameInfoModel model)
    {
        Title = model.Title;
        Content = model.Content;
        Summary = model.Summary;
        StartTimeUTC = model.StartTimeUTC;
        EndTimeUTC = model.EndTimeUTC;
        TeamMemberCountLimit = model.TeamMemberCountLimit;
        ContainerCountLimit = model.ContainerCountLimit;

        return this;
    }
}