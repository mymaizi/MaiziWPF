-- ----------------------------
-- 1、部门表
-- ----------------------------
create table sys_dept (
    dept_id           bigint(20)      not null                   comment '部门id',
    parent_id         bigint(20)      default 0                  comment '父部门id',
    ancestors         varchar(500)    default ''                 comment '祖级列表',
    dept_name         varchar(30)     default ''                 comment '部门名称',
    dept_category     varchar(100)    default null               comment '部门类别编码',
    order_num         int(4)          default 0                  comment '显示顺序',
    leader            bigint(20)      default null               comment '负责人',
    phone             varchar(11)     default null               comment '联系电话',
    email             varchar(50)     default null               comment '邮箱',
    status            char(1)         default '0'                comment '部门状态（0正常 1停用）',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    remark            varchar(500)    default null               comment '备注',
    primary key (dept_id),
    key idx_sys_dept_parent_id (parent_id)
) engine=innodb comment = '部门表';
-- ----------------------------
-- 2、用户信息表
-- ----------------------------
create table sys_user (
    user_id           bigint(20)      not null                   comment '用户ID',
    dept_id           bigint(20)      default null               comment '部门ID',
    user_name         varchar(30)     not null                   comment '用户账号',
    nick_name         varchar(30)     not null                   comment '用户昵称',
    user_type         varchar(10)     default 'sys_user'         comment '用户类型（sys_user系统用户）',
    email             varchar(50)     default ''                 comment '用户邮箱',
    phone_number      varchar(11)     default ''                 comment '手机号码',
    gender            char(1)         default '0'                comment '用户性别（0男 1女 2未知）',
    avatar            bigint(20)                                 comment '头像地址',
    password          varchar(100)    default ''                 comment '密码',
    status            char(1)         default '0'                comment '账号状态（0正常 1停用）',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    login_ip          varchar(128)    default ''                 comment '最后登录IP',
    login_date        datetime                                   comment '最后登录时间',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    remark            varchar(500)    default null               comment '备注',
    primary key (user_id),
    key idx_sys_user_dept_id   (dept_id),
    key idx_sys_user_create_by (create_by),
    key idx_sys_user_user_name (user_name),
    key idx_sys_user_phone     (phone_number)
) engine=innodb comment = '用户信息表';
-- ----------------------------
-- 3、岗位信息表
-- ----------------------------
create table sys_post
(
    post_id       bigint(20)      not null                   comment '岗位ID',
    dept_id       bigint(20)      not null                   comment '部门id',
    post_code     varchar(64)     not null                   comment '岗位编码',
    post_category varchar(100)    default null               comment '岗位类别编码',
    post_name     varchar(50)     not null                   comment '岗位名称',
    post_sort     int(4)          not null                   comment '显示顺序',
    status        char(1)         not null                   comment '状态（0正常 1停用）',
    del_flag      char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    create_dept   bigint(20)      default null               comment '创建部门',
    create_by     bigint(20)      default null               comment '创建者',
    create_time   datetime                                   comment '创建时间',
    update_by     bigint(20)      default null               comment '更新者',
    update_time   datetime                                   comment '更新时间',
    remark        varchar(500)    default null               comment '备注',
    primary key (post_id),
    key idx_sys_post_dept_id (dept_id)
) engine=innodb comment = '岗位信息表';
-- ----------------------------
-- 4、角色信息表
-- ----------------------------
create table sys_role (
    role_id              bigint(20)      not null                   comment '角色ID',
    role_name            varchar(30)     not null                   comment '角色名称',
    role_key             varchar(100)    not null                   comment '角色权限字符串',
    role_sort            int(4)          not null                   comment '显示顺序',
    data_scope           char(1)         default '1'                comment '数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限 5：仅本人数据权限 6：部门及以下或本人数据权限）',
    menu_check_strictly  tinyint(1)      default 1                  comment '菜单树选择项是否关联显示',
    dept_check_strictly  tinyint(1)      default 1                  comment '部门树选择项是否关联显示',
    status               char(1)         not null                   comment '角色状态（0正常 1停用）',
    del_flag             char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    create_dept          bigint(20)      default null               comment '创建部门',
    create_by            bigint(20)      default null               comment '创建者',
    create_time          datetime                                   comment '创建时间',
    update_by            bigint(20)      default null               comment '更新者',
    update_time          datetime                                   comment '更新时间',
    remark               varchar(500)    default null               comment '备注',
    primary key (role_id),
    key idx_sys_role_create_dept (create_dept),
    key idx_sys_role_create_by   (create_by)
) engine=innodb comment = '角色信息表';
-- ----------------------------
-- 5、菜单权限表
-- ----------------------------
create table sys_menu (
    menu_id           bigint(20)      not null                   comment '菜单ID',
    menu_name         varchar(50)     not null                   comment '菜单名称',
    parent_id         bigint(20)      default 0                  comment '父菜单ID',
    order_num         int(4)          default 0                  comment '显示顺序',
    path              varchar(200)    default ''                 comment '路由地址',
    component         varchar(255)    default null               comment '组件路径',
    query_param       varchar(255)    default null               comment '路由参数',
    is_frame          char(1)         default 'N'                comment '是否为外链（Y是 N否）',
    is_cache          char(1)         default 'Y'                comment '是否缓存（Y缓存 N不缓存）',
    menu_type         char(1)         default ''                 comment '菜单类型（M目录 C菜单 F按钮）',
    visible           char(1)         default 0                  comment '显示状态（0显示 1隐藏）',
    status            char(1)         default 0                  comment '菜单状态（0正常 1停用）',
    perms             varchar(100)    default null               comment '权限标识',
    icon              varchar(100)    default '#'                comment '菜单图标',
    active_menu       varchar(255)    default ''                 comment '激活菜单路径',
    ext               varchar(2000)   default ''                 comment '扩展字段',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    remark            varchar(500)    default ''                 comment '备注',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (menu_id)
) engine=innodb comment = '菜单权限表';
-- ==================== 目录（M）====================
-- 1. 系统管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (1, '系统管理', 0, 1, '', '', 'M', '0', '0', '', 'Cog', 'N', 'Y');

-- 2. 系统监控
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (2, '系统监控', 0, 2, '', '', 'M', '0', '0', '', 'MonitorDashboard', 'N', 'Y');

-- 3. 日志管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (3, '日志管理', 0, 3, '', '', 'M', '0', '0', '', 'TextBoxSearch', 'N', 'Y');

-- 4. 文件管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (4, '文件管理', 0, 4, '', '', 'M', '0', '0', '', 'FolderOpen', 'N', 'Y');

-- ==================== 系统管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (100, '用户管理', 1, 1, 'MaiziWPF.Modules.Sys', 'UserListView', 'C', '0', '0', 'system:user:list', 'Account', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (101, '角色管理', 1, 2, 'MaiziWPF.Modules.Sys', 'RoleListView', 'C', '0', '0', 'system:role:list', 'AccountGroup', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (102, '菜单管理', 1, 3, 'MaiziWPF.Modules.Sys', 'MenuListView', 'C', '0', '0', 'system:menu:list', 'Menu', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (103, '部门管理', 1, 4, 'MaiziWPF.Modules.Sys', 'DeptListView', 'C', '0', '0', 'system:dept:list', 'Domain', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (104, '岗位管理', 1, 5, 'MaiziWPF.Modules.Sys', 'PostListView', 'C', '0', '0', 'system:post:list', 'BadgeAccount', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (105, '字典管理', 1, 6, 'MaiziWPF.Modules.Sys', 'DictListView', 'C', '0', '0', 'system:dict:list', 'BookOpenVariant', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (106, '参数设置', 1, 7, 'MaiziWPF.Modules.Sys', 'ConfigListView', 'C', '0', '0', 'system:config:list', 'CogOutline', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (107, '通知公告', 1, 8, 'MaiziWPF.Modules.Sys', 'NoticeListView', 'C', '0', '0', 'system:notice:list', 'BellRing', 'N', 'Y');

-- ==================== 系统监控 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (200, '在线用户', 2, 1, 'MaiziWPF.Modules.Sys', 'OnlineUserListView', 'C', '0', '0', 'monitor:online:list', 'AccountCheck', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (201, '定时任务', 2, 2, 'MaiziWPF.Modules.Sys', 'JobListView', 'C', '0', '0', 'monitor:job:list', 'ClockOutline', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (202, '缓存监控', 2, 3, 'MaiziWPF.Modules.Sys', 'CacheMonitorView', 'C', '0', '0', 'monitor:cache:list', 'Database', 'N', 'Y');

-- ==================== 日志管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (300, '操作日志', 3, 1, 'MaiziWPF.Modules.Sys', 'OperLogListView', 'C', '0', '0', 'monitor:operlog:list', 'TextBox', 'N', 'Y');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (301, '登录日志', 3, 2, 'MaiziWPF.Modules.Sys', 'LoginLogListView', 'C', '0', '0', 'monitor:logininfor:list', 'LoginVariant', 'N', 'Y');

-- ==================== 文件管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache) 
VALUES (400, '文件管理', 4, 1, 'MaiziWPF.Modules.Sys', 'OssListView', 'C', '0', '0', 'system:oss:list', 'FileDocument', 'N', 'Y');
-- ----------------------------
-- 6、用户和角色关联表  用户N-1角色
-- ----------------------------
create table sys_user_role (
    user_id   bigint(20) not null comment '用户ID',
    role_id   bigint(20) not null comment '角色ID',
    primary key(user_id, role_id),
    key idx_sys_user_role_rid (role_id)
) engine=innodb comment = '用户和角色关联表';
-- ----------------------------
-- 7、角色和菜单关联表  角色1-N菜单
-- ----------------------------
create table sys_role_menu (
    role_id   bigint(20) not null comment '角色ID',
    menu_id   bigint(20) not null comment '菜单ID',
    primary key(role_id, menu_id)
) engine=innodb comment = '角色和菜单关联表';
-- ----------------------------
-- 8、角色和部门关联表  角色1-N部门
-- ----------------------------
create table sys_role_dept (
    role_id   bigint(20) not null comment '角色ID',
    dept_id   bigint(20) not null comment '部门ID',
    primary key(role_id, dept_id)
) engine=innodb comment = '角色和部门关联表';
-- ----------------------------
-- 9、用户与岗位关联表  用户1-N岗位
-- ----------------------------
create table sys_user_post
(
    user_id   bigint(20) not null comment '用户ID',
    post_id   bigint(20) not null comment '岗位ID',
    primary key (user_id, post_id)
) engine=innodb comment = '用户与岗位关联表';
-- ----------------------------
-- 10、操作日志记录
-- ----------------------------
create table sys_oper_log (
    oper_id           bigint(20)      not null                   comment '日志主键',
    title             varchar(50)     default ''                 comment '模块标题',
    business_type     int(2)          default 0                  comment '业务类型（0其它 1新增 2修改 3删除）',
    method            varchar(100)    default ''                 comment '方法名称',
    request_method    varchar(10)     default ''                 comment '请求方式',
    operator_type     int(1)          default 0                  comment '操作类别（0其它 1后台用户 2手机端用户）',
    oper_name         varchar(50)     default ''                 comment '操作人员',
    user_id           bigint(20)      default null               comment '操作用户ID',
    dept_id           bigint(20)      default null               comment '操作部门ID',
    dept_name         varchar(50)     default ''                 comment '部门名称',
    client_key        varchar(32)     default ''                 comment '客户端',
    device_type       varchar(32)     default ''                 comment '设备类型',
    browser           varchar(50)     default ''                 comment '浏览器类型',
    os                varchar(50)     default ''                 comment '操作系统',
    oper_url          varchar(255)    default ''                 comment '请求URL',
    oper_ip           varchar(128)    default ''                 comment '主机地址',
    oper_location     varchar(255)    default ''                 comment '操作地点',
    oper_param        varchar(4000)   default ''                 comment '请求参数',
    json_result       varchar(4000)   default ''                 comment '返回参数',
    status            int(1)          default 0                  comment '操作状态（0正常 1异常）',
    error_msg         varchar(4000)   default ''                 comment '错误消息',
    oper_time         datetime                                   comment '操作时间',
    cost_time         bigint(20)      default 0                  comment '消耗时间',
    primary key (oper_id),
    key idx_sys_oper_log_bt (business_type),
    key idx_sys_oper_log_uid (user_id),
    key idx_sys_oper_log_s  (status),
    key idx_sys_oper_log_ot (oper_time)
) engine=innodb comment = '操作日志记录';
-- ----------------------------
-- 11、字典类型表
-- ----------------------------
create table sys_dict_type
(
    dict_id          bigint(20)      not null                   comment '字典主键',
    dict_name        varchar(100)    default ''                 comment '字典名称',
    dict_type        varchar(100)    default ''                 comment '字典类型',
    create_dept      bigint(20)      default null               comment '创建部门',
    create_by        bigint(20)      default null               comment '创建者',
    create_time      datetime                                   comment '创建时间',
    update_by        bigint(20)      default null               comment '更新者',
    update_time      datetime                                   comment '更新时间',
    remark           varchar(500)    default null               comment '备注',
    status           char(1)         default 'N'                comment '是否停用（Y是 N否）',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (dict_id),
    unique (dict_type)
) engine=innodb comment = '字典类型表';
-- ----------------------------
-- 12、字典数据表
-- ----------------------------
create table sys_dict_data
(
    dict_code        bigint(20)      not null                   comment '字典编码',
    dict_sort        int(4)          default 0                  comment '字典排序',
    dict_label       varchar(100)    default ''                 comment '字典标签',
    dict_value       varchar(100)    default ''                 comment '字典键值',
    dict_type        varchar(100)    default ''                 comment '字典类型',
    is_default       char(1)         default 'N'                comment '是否默认（Y是 N否）',
    create_dept      bigint(20)      default null               comment '创建部门',
    create_by        bigint(20)      default null               comment '创建者',
    create_time      datetime                                   comment '创建时间',
    update_by        bigint(20)      default null               comment '更新者',
    update_time      datetime                                   comment '更新时间',
    remark           varchar(500)    default null               comment '备注',
    status           char(1)         default 'N'                comment '是否停用（Y是 N否）',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (dict_code),
    key idx_sys_dict_data_type (dict_type)
) engine=innodb comment = '字典数据表';
-- ----------------------------
-- 13、参数配置表
-- ----------------------------
create table sys_config (
    config_id         bigint(20)      not null                   comment '参数主键',
    config_name       varchar(100)    default ''                 comment '参数名称',
    config_key        varchar(100)    default ''                 comment '参数键名',
    config_value      varchar(500)    default ''                 comment '参数键值',
    config_type       char(1)         default 'N'                comment '系统内置（Y是 N否）',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    remark            varchar(500)    default null               comment '备注',
    primary key (config_id)
) engine=innodb comment = '参数配置表';
-- ----------------------------
-- 14、系统访问记录
-- ----------------------------
create table sys_login_info (
    info_id        bigint(20)     not null                  comment '访问ID',
    user_name      varchar(50)    default ''                comment '用户账号',
    client_key     varchar(32)    default ''                comment '客户端',
    device_type    varchar(32)    default ''                comment '设备类型',
    ipaddr         varchar(128)   default ''                comment '登录IP地址',
    login_location varchar(255)   default ''                comment '登录地点',
    browser        varchar(50)    default ''                comment '浏览器类型',
    os             varchar(50)    default ''                comment '操作系统',
    status         char(1)        default '0'               comment '登录状态（0正常 1异常）',
    msg            varchar(255)   default ''                comment '提示消息',
    login_time     datetime                                 comment '访问时间',
    primary key (info_id),
    key idx_sys_login_info_s  (status),
    key idx_sys_login_info_lt (login_time)
) engine=innodb comment = '系统访问记录';
-- ----------------------------
-- 15、通知公告表
-- ----------------------------
create table sys_notice (
    notice_id         bigint(20)      not null                   comment '公告ID',
    notice_title      varchar(50)     not null                   comment '公告标题',
    notice_type       char(1)         not null                   comment '公告类型（1通知 2公告）',
    notice_content    longblob        default null               comment '公告内容',
    status            char(1)         default '0'                comment '公告状态（0正常 1关闭）',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    remark            varchar(255)    default null               comment '备注',
    primary key (notice_id)
) engine=innodb comment = '通知公告表';
-- ----------------------------
-- 16、消息记录表
-- ----------------------------
create table sys_message (
    message_id        bigint(20)      not null                   comment '消息ID',
    category          varchar(20)     not null                   comment '消息分组(system/notice/workflow)',
    type              varchar(20)     not null                   comment '消息类型',
    source            varchar(20)     not null                   comment '消息来源',
    title             varchar(100)    default ''                 comment '标题',
    message           varchar(500)    default ''                 comment '摘要消息',
    content           longtext                                   comment '详细内容',
    data_json         longtext                                   comment '扩展数据JSON',
    path              varchar(500)    default null               comment '前端跳转路径',
    send_user_ids     varchar(2000)   not null default '0'       comment '目标用户ID串，0表示全局',
    create_dept       bigint(20)      default null               comment '创建部门',
    create_by         bigint(20)      default null               comment '创建者',
    create_time       datetime                                   comment '创建时间',
    update_by         bigint(20)      default null               comment '更新者',
    update_time       datetime                                   comment '更新时间',
    primary key (message_id),
    key idx_sys_message_category_time (category, create_time)
) engine=innodb comment = '消息记录表';
-- ----------------------------
-- 17、OSS对象存储表
-- ----------------------------
create table sys_oss (
    oss_id          bigint(20)   not null                   comment '对象存储主键',
    file_name       varchar(255) not null default ''        comment '文件名',
    original_name   varchar(255) not null default ''        comment '原名',
    file_suffix     varchar(10)  not null default ''        comment '文件后缀名',
    url             varchar(500) not null                   comment 'URL地址',
    ext1            text                  default null      comment '扩展字段',
    create_dept     bigint(20)            default null      comment '创建部门',
    create_time     datetime              default null      comment '创建时间',
    create_by       bigint(20)            default null      comment '上传人',
    update_time     datetime              default null      comment '更新时间',
    update_by       bigint(20)            default null      comment '更新人',
    service         varchar(20)  not null default 'minio'   comment '服务商',
    primary key (oss_id)
) engine=innodb comment ='OSS对象存储表';