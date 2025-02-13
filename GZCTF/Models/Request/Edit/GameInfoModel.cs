﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CTFServer.Models.Request.Edit;

/// <summary>
/// 比赛信息（Edit）
/// </summary>
public class GameInfoModel
{
    /// <summary>
    /// 比赛 Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 比赛标题
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

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
    /// 比赛头图
    /// </summary>
    [JsonPropertyName("poster")]
    public string? PosterUrl { get; set; } = string.Empty;

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

    public static GameInfoModel FromGame(Models.Game game)
        => new()
        {
            Id = game.Id,
            Title = game.Title,
            Summary = game.Summary,
            Content = game.Content,
            PosterUrl = game.PosterUrl,
            TeamMemberCountLimit = game.TeamMemberCountLimit,
            ContainerCountLimit = game.ContainerCountLimit,
            StartTimeUTC = game.StartTimeUTC,
            EndTimeUTC = game.EndTimeUTC
        };
}