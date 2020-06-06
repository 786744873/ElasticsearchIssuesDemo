using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticsearchIssuesDemo
{
    /// <summary>
    /// 存储到ElasticSearch
    /// </summary>
    public class GatewayDevicePoint_ES : ESEntity
    {
        /// <summary>
        /// 存储点的id
        /// </summary>
        public string PointId { get; set; }
        /// <summary>
        /// 实时值
        /// </summary>
        public double Value { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public Int64 TimeTicks { get; set; }
    }

    [ElasticsearchType(IdProperty = "Id")]
    public abstract class ESEntity : ESEntity<string>
    {
        public ESEntity()
        {
            Id = Guid.NewGuid().ToString("N");
            AddTime = DateTime.Now;
            AddTimeTicks = DateTime.Now.Ticks;
        }
    }

    /// <summary>
    /// ES基类
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    public abstract class ESEntity<Key> : IESEntity<Key>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public virtual Key Id { get; set; }

        /// <summary>
        /// 新增时间
        /// </summary>
        [Required]
        public virtual DateTime AddTime { get; set; }

        /// <summary>
        /// 新增时间
        /// </summary>
        public virtual Int64 AddTimeTicks { get; set; }

        /// <summary>
        /// 新增标识
        /// </summary>
        [MaxLength(32)]
        public virtual string AddIdentityId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 修改标识
        /// </summary>
        [MaxLength(32)]
        public virtual string ModifyIdentityId { get; set; }

        /// <summary>
        /// 删除 0=不删除/1=删除
        /// </summary>
        public virtual bool IsDelete { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public virtual DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除标识
        /// </summary>
        [MaxLength(32)]
        public virtual string DeleteIdentityId { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public virtual long Sort { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [MaxLength(255)]
        public virtual string Remark { get; set; }
    }

    /// <summary>
    /// ES基类约束
    /// </summary>
    public interface IESEntity<TKey>
    {
        /// <summary>
        /// Id
        /// </summary>
        TKey Id { get; set; }

        /// <summary>
        /// 新增时间
        /// </summary>
        DateTime AddTime { get; set; }

        /// <summary>
        /// 新增时间Ticks
        /// </summary>
        Int64 AddTimeTicks { get; set; }

        /// <summary>
        /// 新增标识
        /// </summary>
        string AddIdentityId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 修改标识
        /// </summary>
        string ModifyIdentityId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>

        DateTime? DeleteTime { get; set; }
        /// <summary>
        /// 删除标识
        /// </summary>
        string DeleteIdentityId { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        string Remark { get; set; }
    }

    /// <summary>
    /// ES基类约束
    /// </summary>
    public interface IESEntity : IESEntity<string>
    { }
}
