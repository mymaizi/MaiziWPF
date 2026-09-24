-- ----------------------------
-- 1、部门表
-- ----------------------------
DROP TABLE IF EXISTS sys_dept;
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
DROP TABLE IF EXISTS sys_user;
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
INSERT INTO sys_user (user_id, dept_id, user_name, nick_name, user_type, email, phone_number, gender, avatar, password, status, del_flag, login_ip, login_date, create_dept, create_by, create_time, update_by, update_time, remark) VALUES (1761100000000000001, 1761000000000000103, 'admin', '麦子', 'sys_user', '', '', '1', 0, '$2a$10$7JB720yubVSZvUI0rEqK/.VqGOZTH.ulu33dHOiBE8ByOhJIrdAu2', '0', '0', '127.0.0.1', '2026-09-10 16:46:14.000', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:14.000', 0, '0001-01-01 00:00:00.000', '管理员');
-- ----------------------------
-- 3、岗位信息表
-- ----------------------------
DROP TABLE IF EXISTS sys_post;
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
DROP TABLE IF EXISTS sys_role;
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
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, del_flag, create_dept, create_by, create_time, update_by, update_time, remark) VALUES (1761300000000000001, '超级管理员', 'superadmin', 1, '1', 1, 1, '0', '0', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:14.000', 0, '0001-01-01 00:00:00.000', '超级管理员');
-- ----------------------------
-- 5、菜单权限表
-- ----------------------------
DROP TABLE IF EXISTS sys_menu;
create table sys_menu (
    menu_id           bigint(20)      not null                   comment '菜单ID',
    menu_name         varchar(50)     not null                   comment '菜单名称',
    parent_id         bigint(20)      default 0                  comment '父菜单ID',
    order_num         int(4)          default 0                  comment '显示顺序',
    path              varchar(200)    default ''                 comment '路由地址',
    component         varchar(255)    default null               comment '组件路径',
    query_param       varchar(255)    default null               comment '路由参数',
    is_frame          char(1)         default 'N'                comment '是否为外链（Y是 N否）',
    menu_type         char(1)         default ''                 comment '菜单类型（M目录 C菜单 F按钮）',
    status            char(1)         default 0                  comment '菜单状态（0正常 1停用）',
    perms             varchar(100)    default null               comment '权限标识',
    icon              varchar(100)    default '#'                comment '菜单图标',
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
-- 1. 首页
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (1, '首页', 0, 0, 'MaiziWPF.Modules.Sys', 'DashboardView', NULL, 'N', 'C', '0', '', 'Home', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 目录（M）====================
-- 2. 系统管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2, '系统管理', 0, 1, '', '', NULL, 'N', 'M', '0', '', 'Cog', 0, 1, NOW(), 1, NOW(), '', '0');

-- 3. 日志管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3, '日志管理', 0, 2, '', '', NULL, 'N', 'M', '0', '', 'TextBoxSearch', 0, 1, NOW(), 1, NOW(), '', '0');

-- 4. 文件管理
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4, '文件管理', 0, 3, '', '', NULL, 'N', 'M', '0', '', 'FolderOpen', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 系统管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (200, '用户管理', 2, 1, 'MaiziWPF.Modules.Sys', 'UserListView', NULL, 'N', 'C', '0', 'system:user:list', 'Account', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (201, '角色管理', 2, 2, 'MaiziWPF.Modules.Sys', 'RoleListView', NULL, 'N', 'C', '0', 'system:role:list', 'AccountGroup', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (202, '菜单管理', 2, 3, 'MaiziWPF.Modules.Sys', 'MenuListView', NULL, 'N', 'C', '0', 'system:menu:list', 'Menu', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (203, '部门管理', 2, 4, 'MaiziWPF.Modules.Sys', 'DeptListView', NULL, 'N', 'C', '0', 'system:dept:list', 'Domain', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (204, '岗位管理', 2, 5, 'MaiziWPF.Modules.Sys', 'PostListView', NULL, 'N', 'C', '0', 'system:post:list', 'BadgeAccount', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (205, '字典管理', 2, 6, 'MaiziWPF.Modules.Sys', 'DictListView', NULL, 'N', 'C', '0', 'system:dict:list', 'BookOpenVariant', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (206, '参数设置', 2, 7, 'MaiziWPF.Modules.Sys', 'ConfigListView', NULL, 'N', 'C', '0', 'system:config:list', 'CogOutline', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (207, '通知公告', 2, 8, 'MaiziWPF.Modules.Sys', 'NoticeListView', NULL, 'N', 'C', '0', 'system:notice:list', 'BellRing', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 日志管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (300, '操作日志', 3, 1, 'MaiziWPF.Modules.Sys', 'OperLogListView', NULL, 'N', 'C', '0', 'monitor:operlog:list', 'TextBox', 0, 1, NOW(), 1, NOW(), '', '0');

INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (301, '登录日志', 3, 2, 'MaiziWPF.Modules.Sys', 'LoginInfoListView', NULL, 'N', 'C', '0', 'monitor:logininfor:list', 'LoginVariant', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 文件管理 子菜单（C）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (400, '文件管理', 4, 1, 'MaiziWPF.Modules.Sys', 'OssListView', NULL, 'N', 'C', '0', 'system:oss:list', 'FileDocument', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 用户管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2001, '用户查询', 200, 1, '', '', NULL, 'N', 'F', '0', 'system:user:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2002, '用户新增', 200, 2, '', '', NULL, 'N', 'F', '0', 'system:user:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2003, '用户修改', 200, 3, '', '', NULL, 'N', 'F', '0', 'system:user:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2004, '用户删除', 200, 4, '', '', NULL, 'N', 'F', '0', 'system:user:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2005, '重置密码', 200, 5, '', '', NULL, 'N', 'F', '0', 'system:user:resetPwd', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2006, '分配角色', 200, 6, '', '', NULL, 'N', 'F', '0', 'system:user:assignRole', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 角色管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2011, '角色查询', 201, 1, '', '', NULL, 'N', 'F', '0', 'system:role:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2012, '角色新增', 201, 2, '', '', NULL, 'N', 'F', '0', 'system:role:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2013, '角色修改', 201, 3, '', '', NULL, 'N', 'F', '0', 'system:role:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2014, '角色删除', 201, 4, '', '', NULL, 'N', 'F', '0', 'system:role:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2015, '分配权限', 201, 5, '', '', NULL, 'N', 'F', '0', 'system:role:assignPerms', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2016, '分配用户', 201, 6, '', '', NULL, 'N', 'F', '0', 'system:role:assignUser', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 菜单管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2021, '菜单查询', 202, 1, '', '', NULL, 'N', 'F', '0', 'system:menu:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2022, '菜单新增', 202, 2, '', '', NULL, 'N', 'F', '0', 'system:menu:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2023, '菜单修改', 202, 3, '', '', NULL, 'N', 'F', '0', 'system:menu:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2024, '菜单删除', 202, 4, '', '', NULL, 'N', 'F', '0', 'system:menu:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 部门管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2031, '部门查询', 203, 1, '', '', NULL, 'N', 'F', '0', 'system:dept:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2032, '部门新增', 203, 2, '', '', NULL, 'N', 'F', '0', 'system:dept:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2033, '部门修改', 203, 3, '', '', NULL, 'N', 'F', '0', 'system:dept:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2034, '部门删除', 203, 4, '', '', NULL, 'N', 'F', '0', 'system:dept:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 岗位管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2041, '岗位查询', 204, 1, '', '', NULL, 'N', 'F', '0', 'system:post:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2042, '岗位新增', 204, 2, '', '', NULL, 'N', 'F', '0', 'system:post:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2043, '岗位修改', 204, 3, '', '', NULL, 'N', 'F', '0', 'system:post:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2044, '岗位删除', 204, 4, '', '', NULL, 'N', 'F', '0', 'system:post:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 字典管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2051, '字典查询', 205, 1, '', '', NULL, 'N', 'F', '0', 'system:dict:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2052, '字典新增', 205, 2, '', '', NULL, 'N', 'F', '0', 'system:dict:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2053, '字典修改', 205, 3, '', '', NULL, 'N', 'F', '0', 'system:dict:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2054, '字典删除', 205, 4, '', '', NULL, 'N', 'F', '0', 'system:dict:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 参数设置 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2061, '参数查询', 206, 1, '', '', NULL, 'N', 'F', '0', 'system:config:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2062, '参数新增', 206, 2, '', '', NULL, 'N', 'F', '0', 'system:config:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2063, '参数修改', 206, 3, '', '', NULL, 'N', 'F', '0', 'system:config:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2064, '参数删除', 206, 4, '', '', NULL, 'N', 'F', '0', 'system:config:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 通知公告 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2071, '公告查询', 207, 1, '', '', NULL, 'N', 'F', '0', 'system:notice:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2072, '公告新增', 207, 2, '', '', NULL, 'N', 'F', '0', 'system:notice:add', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2073, '公告修改', 207, 3, '', '', NULL, 'N', 'F', '0', 'system:notice:edit', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (2074, '公告删除', 207, 4, '', '', NULL, 'N', 'F', '0', 'system:notice:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 操作日志 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3001, '日志查询', 300, 1, '', '', NULL, 'N', 'F', '0', 'monitor:operlog:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3002, '日志删除', 300, 2, '', '', NULL, 'N', 'F', '0', 'monitor:operlog:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3003, '日志清空', 300, 3, '', '', NULL, 'N', 'F', '0', 'monitor:operlog:clean', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3004, '日志导出', 300, 4, '', '', NULL, 'N', 'F', '0', 'monitor:operlog:export', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 登录日志 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3011, '日志查询', 301, 1, '', '', NULL, 'N', 'F', '0', 'monitor:logininfor:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3012, '日志删除', 301, 2, '', '', NULL, 'N', 'F', '0', 'monitor:logininfor:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3013, '日志清空', 301, 3, '', '', NULL, 'N', 'F', '0', 'monitor:logininfor:clean', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (3014, '日志导出', 301, 4, '', '', NULL, 'N', 'F', '0', 'monitor:logininfor:export', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ==================== 文件管理 按钮（F）====================
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4001, '文件查询', 400, 1, '', '', NULL, 'N', 'F', '0', 'system:oss:query', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4002, '文件上传', 400, 2, '', '', NULL, 'N', 'F', '0', 'system:oss:upload', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4003, '文件删除', 400, 3, '', '', NULL, 'N', 'F', '0', 'system:oss:remove', '#', 0, 1, NOW(), 1, NOW(), '', '0');
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, component, query_param, is_frame, menu_type, status, perms, icon, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) 
VALUES (4004, '文件下载', 400, 4, '', '', NULL, 'N', 'F', '0', 'system:oss:download', '#', 0, 1, NOW(), 1, NOW(), '', '0');

-- ----------------------------
-- 6、用户和角色关联表  用户N-1角色
-- ----------------------------
DROP TABLE IF EXISTS sys_user_role;
create table sys_user_role (
    user_id   bigint(20) not null comment '用户ID',
    role_id   bigint(20) not null comment '角色ID',
    primary key(user_id, role_id),
    key idx_sys_user_role_rid (role_id)
) engine=innodb comment = '用户和角色关联表';
-- ----------------------------
-- 7、角色和菜单关联表  角色1-N菜单
-- ----------------------------
DROP TABLE IF EXISTS sys_role_menu;
create table sys_role_menu (
    role_id   bigint(20) not null comment '角色ID',
    menu_id   bigint(20) not null comment '菜单ID',
    primary key(role_id, menu_id)
) engine=innodb comment = '角色和菜单关联表';
-- ----------------------------
-- 8、角色和部门关联表  角色1-N部门
-- ----------------------------
DROP TABLE IF EXISTS sys_role_dept;
create table sys_role_dept (
    role_id   bigint(20) not null comment '角色ID',
    dept_id   bigint(20) not null comment '部门ID',
    primary key(role_id, dept_id)
) engine=innodb comment = '角色和部门关联表';
-- ----------------------------
-- 9、用户与岗位关联表  用户1-N岗位
-- ----------------------------
DROP TABLE IF EXISTS sys_user_post;
create table sys_user_post
(
    user_id   bigint(20) not null comment '用户ID',
    post_id   bigint(20) not null comment '岗位ID',
    primary key (user_id, post_id)
) engine=innodb comment = '用户与岗位关联表';
-- ----------------------------
-- 10、操作日志记录
-- ----------------------------
DROP TABLE IF EXISTS sys_oper_log;
create table sys_oper_log (
    oper_id           bigint(20)      not null                   comment '日志主键',
    title             varchar(50)     default ''                 comment '模块标题',
    business_type     int(2)          default 0                  comment '业务类型（0其它 1新增 2修改 3删除）',
    method            varchar(100)    default ''                 comment '方法名称',
    oper_name         varchar(50)     default ''                 comment '操作人员',
    user_id           bigint(20)      default null               comment '操作用户ID',
    dept_id           bigint(20)      default null               comment '操作部门ID',
    dept_name         varchar(50)     default ''                 comment '部门名称',
    client_version    varchar(32)     default ''                 comment '客户端版本',
    mac_address       varchar(50)     default ''                 comment 'MAC地址',
    os                varchar(50)     default ''                 comment '操作系统',
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
DROP TABLE IF EXISTS sys_dict_type;
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
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (dict_id),
    unique (dict_type)
) engine=innodb comment = '字典类型表';
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000001, '用户性别', 'sys_user_gender', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '用户性别列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000002, '菜单状态', 'sys_show_hide', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '菜单状态列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000003, '系统开关', 'sys_normal_disable', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '系统开关列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000006, '系统是否', 'sys_yes_no', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '系统是否列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000007, '通知类型', 'sys_notice_type', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '通知类型列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000008, '通知状态', 'sys_notice_status', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '通知状态列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000009, '操作类型', 'sys_oper_type', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '操作类型列表', '0');
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761500000000000010, '系统状态', 'sys_common_status', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '登录状态列表', '0');
-- ----------------------------
-- 12、字典数据表
-- ----------------------------
DROP TABLE IF EXISTS sys_dict_data;
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
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (dict_code),
    key idx_sys_dict_data_type (dict_type)
) engine=innodb comment = '字典数据表';
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000001, 1, '男', '0', 'sys_user_gender', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '性别男', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000002, 2, '女', '1', 'sys_user_gender', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '性别女', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000003, 3, '未知', '2', 'sys_user_gender', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '性别未知', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000004, 1, '显示', '0', 'sys_show_hide', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '显示菜单', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000005, 2, '隐藏', '1', 'sys_show_hide', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '隐藏菜单', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000006, 1, '正常', '0', 'sys_normal_disable', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '正常状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000007, 2, '停用', '1', 'sys_normal_disable', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '停用状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000012, 1, '是', 'Y', 'sys_yes_no', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '系统默认是', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000013, 2, '否', 'N', 'sys_yes_no', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '系统默认否', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000014, 1, '通知', '1', 'sys_notice_type', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '通知', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000015, 2, '公告', '2', 'sys_notice_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '公告', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000016, 1, '正常', '0', 'sys_notice_status', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '正常状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000017, 2, '关闭', '1', 'sys_notice_status', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '关闭状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000018, 1, '新增', '1', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '新增操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000019, 2, '修改', '2', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '修改操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000020, 3, '删除', '3', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '删除操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000021, 4, '授权', '4', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '授权操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000022, 5, '导出', '5', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '导出操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000023, 6, '导入', '6', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '导入操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000024, 7, '强退', '7', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '强退操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000026, 9, '清空数据', '9', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '清空操作', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000027, 1, '成功', '0', 'sys_common_status', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '正常状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000028, 2, '失败', '1', 'sys_common_status', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '停用状态', '0');
INSERT INTO sys_dict_data (dict_code, dict_sort, dict_label, dict_value, dict_type, is_default, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761600000000000029, 99, '其他', '0', 'sys_oper_type', 'N', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '其他操作', '0');
-- ----------------------------
-- 13、参数配置表
-- ----------------------------
DROP TABLE IF EXISTS sys_config;
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
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (config_id)
) engine=innodb comment = '参数配置表';
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761700000000000001, '用户管理-账号初始密码', 'sys.user.initPassword', '123456', 'Y', 1761000000000000103, 1761100000000000001, '2026-09-10 16:46:15.000', 0, '0001-01-01 00:00:00.000', '初始化密码 123456', '0');
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, create_dept, create_by, create_time, update_by, update_time, remark, del_flag) VALUES (1761700000000000003, 'OSS预览列表资源开关', 'sys.oss.previewListResource', 'true', 'Y', 0, 0, '0001-01-01 00:00:00.000', 0, '2026-09-21 16:09:54.472', 'true:开启, false:关闭', '0');
-- ----------------------------
-- 14、系统访问记录
-- ----------------------------
DROP TABLE IF EXISTS sys_login_info;
create table sys_login_info (
    info_id        bigint(20)     not null                  comment '访问ID',
    user_name      varchar(50)    default ''                comment '用户账号',
    client_version varchar(32)    default ''                comment '客户端版本',
    ipaddr         varchar(128)   default ''                comment '登录IP地址',
    login_location varchar(255)   default ''                comment '登录地点',
    mac_address    varchar(50)    default ''                comment 'MAC地址',
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
DROP TABLE IF EXISTS sys_notice;
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
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (notice_id)
) engine=innodb comment = '通知公告表';
-- ----------------------------
-- 16、消息记录表
-- ----------------------------
DROP TABLE IF EXISTS sys_message;
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
    remark            varchar(255)    default null               comment '备注',
    del_flag          char(1)         default '0'                comment '删除标志（0代表存在 1代表删除）',
    primary key (message_id),
    key idx_sys_message_category_time (category, create_time)
) engine=innodb comment = '消息记录表';
-- ----------------------------
-- 17、OSS对象存储表
-- ----------------------------
DROP TABLE IF EXISTS sys_oss;
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
    remark            varchar(255)    default null          comment '备注',
    del_flag          char(1)         default '0'           comment '删除标志（0代表存在 1代表删除）',
    primary key (oss_id)
) engine=innodb comment ='OSS对象存储表';