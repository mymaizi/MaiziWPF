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
-- ==================== 首页（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (1, '首页', 0, 0, 'MaiziWPF.Modules.Sys', 'DashboardView', 'C', '0', '0', '', 'Home', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 目录（M）====================
-- 2. 系统管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2, '系统管理', 0, 1, '', '', 'M', '0', '0', '', 'Cog', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- 4. 日志管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3, '日志管理', 0, 2, '', '', 'M', '0', '0', '', 'TextBoxSearch', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- 5. 文件管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4, '文件管理', 0, 3, '', '', 'M', '0', '0', '', 'FolderOpen', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 系统管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (200, '用户管理', 2, 1, 'MaiziWPF.Modules.Sys', 'UserListView', 'C', '0', '0', 'system:user:list', 'Account', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (201, '角色管理', 2, 2, 'MaiziWPF.Modules.Sys', 'RoleListView', 'C', '0', '0', 'system:role:list', 'AccountGroup', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (202, '菜单管理', 2, 3, 'MaiziWPF.Modules.Sys', 'MenuListView', 'C', '0', '0', 'system:menu:list', 'Menu', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (203, '部门管理', 2, 4, 'MaiziWPF.Modules.Sys', 'DeptListView', 'C', '0', '0', 'system:dept:list', 'Domain', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (204, '岗位管理', 2, 5, 'MaiziWPF.Modules.Sys', 'PostListView', 'C', '0', '0', 'system:post:list', 'BadgeAccount', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (205, '字典管理', 2, 6, 'MaiziWPF.Modules.Sys', 'DictListView', 'C', '0', '0', 'system:dict:list', 'BookOpenVariant', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (206, '参数设置', 2, 7, 'MaiziWPF.Modules.Sys', 'ConfigListView', 'C', '0', '0', 'system:config:list', 'CogOutline', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (207, '通知公告', 2, 8, 'MaiziWPF.Modules.Sys', 'NoticeListView', 'C', '0', '0', 'system:notice:list', 'BellRing', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 日志管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (300, '操作日志', 3, 1, 'MaiziWPF.Modules.Sys', 'OperLogListView', 'C', '0', '0', 'monitor:operlog:list', 'TextBox', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (301, '登录日志', 3, 2, 'MaiziWPF.Modules.Sys', 'LoginLogListView', 'C', '0', '0', 'monitor:logininfor:list', 'LoginVariant', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 文件管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, menu_type, visible, status, perms, icon, is_frame, is_cache, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (400, '文件管理', 4, 1, 'MaiziWPF.Modules.Sys', 'OssListView', 'C', '0', '0', 'system:oss:list', 'FileDocument', 'N', 'Y', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 用户管理 按钮（F）====================
insert into sys_menu values(2001, '用户查询', 200, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2002, '用户新增', 200, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2003, '用户修改', 200, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2004, '用户删除', 200, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2005, '重置密码', 200, 5, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:resetPwd', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2006, '分配角色', 200, 6, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:user:assignRole', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 角色管理 按钮（F）====================
insert into sys_menu values(2011, '角色查询', 201, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2012, '角色新增', 201, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2013, '角色修改', 201, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2014, '角色删除', 201, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2015, '分配权限', 201, 5, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:assignPerms', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2016, '分配用户', 201, 6, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:role:assignUser', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 菜单管理 按钮（F）====================
insert into sys_menu values(2021, '菜单查询', 202, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:menu:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2022, '菜单新增', 202, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:menu:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2023, '菜单修改', 202, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:menu:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2024, '菜单删除', 202, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:menu:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 部门管理 按钮（F）====================
insert into sys_menu values(2031, '部门查询', 203, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dept:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2032, '部门新增', 203, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dept:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2033, '部门修改', 203, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dept:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2034, '部门删除', 203, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dept:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 岗位管理 按钮（F）====================
insert into sys_menu values(2041, '岗位查询', 204, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:post:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2042, '岗位新增', 204, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:post:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2043, '岗位修改', 204, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:post:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2044, '岗位删除', 204, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:post:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 字典管理 按钮（F）====================
insert into sys_menu values(2051, '字典查询', 205, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dict:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2052, '字典新增', 205, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dict:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2053, '字典修改', 205, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dict:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2054, '字典删除', 205, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:dict:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 参数设置 按钮（F）====================
insert into sys_menu values(2061, '参数查询', 206, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:config:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2062, '参数新增', 206, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:config:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2063, '参数修改', 206, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:config:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2064, '参数删除', 206, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:config:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 通知公告 按钮（F）====================
insert into sys_menu values(2071, '公告查询', 207, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:notice:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2072, '公告新增', 207, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:notice:add', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2073, '公告修改', 207, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:notice:edit', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(2074, '公告删除', 207, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:notice:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 操作日志 按钮（F）====================
insert into sys_menu values(3001, '日志查询', 300, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:operlog:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3002, '日志删除', 300, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:operlog:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3003, '日志清空', 300, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:operlog:clean', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3004, '日志导出', 300, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:operlog:export', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 登录日志 按钮（F）====================
insert into sys_menu values(3011, '日志查询', 301, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:logininfor:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3012, '日志删除', 301, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:logininfor:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3013, '日志清空', 301, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:logininfor:clean', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(3014, '日志导出', 301, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'monitor:logininfor:export', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 文件管理 按钮（F）====================
insert into sys_menu values(4001, '文件查询', 400, 1, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:oss:query', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(4002, '文件上传', 400, 2, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:oss:upload', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(4003, '文件删除', 400, 3, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:oss:remove', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');
insert into sys_menu values(4004, '文件下载', 400, 4, '', '', '', 'N', 'Y', 'F', '0', '0', 'system:oss:download', '#', '', '', 0, 1, NOW(), 1, NOW(), '', '0');

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