﻿using CTFServer.Models.Data;
using CTFServer.Models.Request.Edit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CTFServer.Models;

public class Challenge
{
    [Key]
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// 题目名称
    /// </summary>
    [Required(ErrorMessage = "标题是必需的")]
    [MinLength(1, ErrorMessage = "标题过短")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 题目内容
    /// </summary>
    [Required(ErrorMessage = "题目内容是必需的")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用题目
    /// </summary>
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// 题目标签
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChallengeTag Tag { get; set; } = ChallengeTag.Misc;

    /// <summary>
    /// 题目类型，创建后不可更改
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChallengeType Type { get; set; } = ChallengeType.StaticAttachment;

    /// <summary>
    /// 题目提示，用";"分隔
    /// </summary>
    public string Hints { get; set; } = string.Empty;

    /// <summary>
    /// 镜像名称与标签
    /// </summary>
    public string? ContainerImage { get; set; } = string.Empty;

    /// <summary>
    /// 运行内存限制 (MB)
    /// </summary>
    public int? MemoryLimit { get; set; } = 64;

    /// <summary>
    /// CPU 运行数量限制
    /// </summary>
    public int? CPUCount { get; set; } = 1;

    /// <summary>
    /// 镜像暴露端口
    /// </summary>
    public int? ContainerExposePort { get; set; } = 80;

    /// <summary>
    /// 解决题目人数
    /// </summary>
    [Required]
    public int AcceptedCount { get; set; } = 0;

    /// <summary>
    /// 提交答案的数量
    /// </summary>
    [Required]
    [JsonIgnore]
    public int SubmissionCount { get; set; } = 0;

    /// <summary>
    /// 初始分数
    /// </summary>
    [Required]
    public int OriginalScore { get; set; } = 500;

    /// <summary>
    /// 最低分数比例
    /// </summary>
    [Required]
    [Range(0, 1)]
    public double MinScoreRate { get; set; } = 0.25;

    /// <summary>
    /// 难度系数
    /// </summary>
    [Required]
    public double Difficulty { get; set; } = 5;

    /// <summary>
    /// 当前题目分值
    /// </summary>
    [NotMapped]
    public int CurrentScore =>
        AcceptedCount <= 1 ? OriginalScore : (int)Math.Floor(
        OriginalScore * (MinScoreRate +
            (1.0 - MinScoreRate) * Math.Exp((1 - AcceptedCount) / Difficulty)
        ));

    /// <summary>
    /// 下载文件名称，仅用于动态附件统一文件名
    /// </summary>
    public string? FileName { get; set; } = "attachment";

    #region Db Relationship

    /// <summary>
    /// 题目附件 Id
    /// </summary>
    public int? AttachmentId { get; set; }

    /// <summary>
    /// 题目附件（动态附件存放于 FlagContext）
    /// </summary>
    public Attachment? Attachment { get; set; }

    /// <summary>
    /// 测试容器 Id
    /// </summary>
    public string? TestContainerId { get; set; }

    /// <summary>
    /// 测试容器
    /// </summary>
    public Container? TestContainer { get; set; }

    /// <summary>
    /// 题目对应的 Flag 列表
    /// </summary>
    public List<FlagContext> Flags { get; set; } = new();

    /// <summary>
    /// 提交
    /// </summary>
    public List<Submission> Submissions { get; set; } = new();

    /// <summary>
    /// 赛题实例
    /// </summary>
    public List<Instance> Instances { get; set; } = new();

    /// <summary>
    /// 激活赛题的队伍
    /// </summary>
    public HashSet<Participation> Teams { get; set; } = new();

    /// <summary>
    /// 比赛 Id
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// 比赛对象
    /// </summary>
    public Game Game { get; set; } = default!;

    #endregion Db Relationship

    public Challenge Update(ChallengeUpdateModel model)
    {
        Title = model.Title ?? Title;
        Content = model.Content ?? Content;
        Tag = model.Tag ?? Tag;
        Hints = model.Hints ?? Hints;
        IsEnabled = model.IsEnabled ?? IsEnabled;
        ContainerImage = model.ContainerImage ?? ContainerImage;
        MemoryLimit = model.MemoryLimit ?? MemoryLimit;
        CPUCount = model.CPUCount ?? CPUCount;
        ContainerExposePort = model.ContainerExposePort ?? ContainerExposePort;
        OriginalScore = model.OriginalScore ?? OriginalScore;
        MinScoreRate = model.MinScoreRate ?? MinScoreRate;
        Difficulty = model.Difficulty ?? Difficulty;
        FileName = model.FileName ?? FileName;

        return this;
    }
}