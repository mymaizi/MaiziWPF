using FreeSql.DataAnnotations;
using System.ComponentModel;

namespace MaiziWPF.Services.Domain
{
    /// <summary>
    /// 菜单权限表 sys_menu
    /// </summary>
    [Table(Name = "sys_menu")]
    public class SysMenu : BaseEntity, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isMenuSelected;
        /// <summary>
        /// 菜单是否选中（UI绑定用，不映射到数据库）
        /// </summary>
        [Column(IsIgnore = true)]
        public bool IsMenuSelected
        {
            get => _isMenuSelected;
            set
            {
                if (_isMenuSelected != value)
                {
                    _isMenuSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMenuSelected)));
                }
            }
        }
        /// <summary>
        /// 菜单ID
        /// </summary>
        [Column(Name = "menu_id", IsIdentity = true, IsPrimary = true)]
        public Int64 Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Column(Name = "menu_name", DbType = "varchar(50)")]
        public String MenuName { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        [Column(Name = "parent_id")]
        public Int64 ParentId { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [Column(Name = "order_num")]
        public Int32 OrderNum { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        [Column(Name = "path", DbType = "varchar(200)")]
        public String Path { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        [Column(Name = "component", DbType = "varchar(255)")]
        public String Component { get; set; }

        /// <summary>
        /// 路由参数
        /// </summary>
        [Column(Name = "query_param", DbType = "varchar(255)")]
        public String QueryParam { get; set; }

        /// <summary>
        /// 是否为外链（Y是 N否）
        /// </summary>
        [Column(Name = "is_frame", DbType = "char(1)")]
        public String IsFrame { get; set; }

        /// <summary>
        /// 菜单类型（M目录 C菜单 F按钮）
        /// </summary>
        [Column(Name = "menu_type", DbType = "char(1)")]
        public String MenuType { get; set; }

        /// <summary>
        /// 菜单状态（0正常 1停用）
        /// </summary>
        [Column(Name = "status", DbType = "char(1)")]
        public String Status { get; set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        [Column(Name = "perms", DbType = "varchar(100)")]
        public String Perms { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [Column(Name = "icon", DbType = "varchar(100)")]
        public String Icon { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        [Navigate(nameof(ParentId))]
        public List<SysMenu> Childs { get; set; }

        /// <summary>
        /// 层级（不映射到数据库，由代码计算）
        /// </summary>
        [Column(IsIgnore = true)]
        public Int32 Level { get; set; }

    }
}