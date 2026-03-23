<template>
    <div class="ftp-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header>
                    <span style="font-weight: bold;">选择煤矿</span>
                </template>
                <el-tree 
                    :data="state.treeData" 
                    :props="state.treeProps"
                    @node-click="handleNodeClick"
                    node-key="id"
                    default-expand-all
                    highlight-current
                />
            </el-card>
        </div>
        <div class="right-content">
            <el-form :inline="true" :model="state.queryParams" class="search-form">
                <el-form-item label="用户名">
                    <el-input v-model="state.queryParams.username" placeholder="请输入用户名" clearable @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item label="绑定系统">
                    <el-select v-model="state.queryParams.bindSystem" placeholder="请选择" clearable>
                        <el-option label="安全监测" value="安全监测" />
                        <el-option label="人员定位" value="人员定位" />
                        <el-option label="矿压监测" value="矿压监测" />
                        <el-option label="水文监测" value="水文监测" />
                    </el-select>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleQuery">查询</el-button>
                    <el-button @click="resetQuery">重置</el-button>
                    <el-button type="primary" @click="openAdd">新增</el-button>
                </el-form-item>
            </el-form>

            <el-table v-loading="state.loading" :data="state.tableData" border stripe>
                <el-table-column type="index" label="序号" width="60" align="center" />
                <el-table-column prop="mineName" label="所属煤矿" min-width="120" align="center" />
                <el-table-column prop="host" label="FTP地址" min-width="120" align="center" />
                <el-table-column prop="port" label="端口" width="80" align="center" />
                <el-table-column prop="username" label="FTP用户名" min-width="100" align="center" />
                <el-table-column prop="rootDirectory" label="根目录" min-width="120" align="center" show-overflow-tooltip />
                <el-table-column prop="bindSystem" label="绑定系统" width="100" align="center" />
                <el-table-column label="状态" width="80" align="center">
                    <template #default="scope">
                        <el-tag :type="scope.row.enabled === 1 ? 'success' : 'danger'">
                            {{ scope.row.enabled === 1 ? '启用' : '禁用' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="180" align="center" fixed="right">
                    <template #default="scope">
                        <el-button type="primary" link @click="openEdit(scope.row)">编辑</el-button>
                        <el-button type="danger" link @click="delData(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>

            <el-pagination
                v-model:current-page="state.queryParams.page"
                v-model:page-size="state.queryParams.pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="getList"
                @current-change="getList"
            />

            <el-dialog v-model="state.dialogVisible" :title="state.isEdit ? '编辑FTP配置' : '新增FTP配置'" width="600px">
                <el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px">
                    <el-form-item label="煤矿" prop="mineId">
                        <el-select v-model="state.form.mineId" placeholder="请选择煤矿">
                            <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="FTP地址" prop="host">
                        <el-input v-model="state.form.host" placeholder="请输入FTP地址" />
                    </el-form-item>
                    <el-form-item label="端口" prop="port">
                        <el-input-number v-model="state.form.port" :min="1" :max="65535" />
                    </el-form-item>
                    <el-form-item label="用户名" prop="username">
                        <el-input v-model="state.form.username" placeholder="请输入用户名" />
                    </el-form-item>
                    <el-form-item label="密码" prop="password">
                        <el-input v-model="state.form.password" type="password" placeholder="请输入密码" show-password />
                    </el-form-item>
                    <el-form-item label="根目录" prop="rootDirectory">
                        <el-input v-model="state.form.rootDirectory" placeholder="请输入根目录" />
                    </el-form-item>
                    <el-form-item label="绑定系统" prop="bindSystem">
                        <el-select v-model="state.form.bindSystem" placeholder="请选择绑定系统">
                            <el-option label="安全监测" value="安全监测" />
                            <el-option label="人员定位" value="人员定位" />
                            <el-option label="矿压监测" value="矿压监测" />
                            <el-option label="水文监测" value="水文监测" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="启用">
                        <el-switch v-model="state.form.enabled" :active-value="1" :inactive-value="0" />
                    </el-form-item>
                    <el-form-item label="备注">
                        <el-input v-model="state.form.remark" type="textarea" :rows="2" />
                    </el-form-item>
                </el-form>
                <template #footer>
                    <el-button @click="state.dialogVisible = false">取消</el-button>
                    <el-button type="primary" @click="submitForm" :loading="state.submitLoading">确定</el-button>
                </template>
            </el-dialog>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, FtpConfigApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'label' },
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        username: '',
        bindSystem: ''
    },
    loading: false,
    total: 0,
    tableData: [] as any[],
    mineList: [] as any[],
    dialogVisible: false,
    isEdit: false,
    submitLoading: false,
    form: {} as any
})

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    host: [{ required: true, message: '请输入FTP地址', trigger: 'blur' }],
    port: [{ required: true, message: '请输入端口', trigger: 'blur' }],
    username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
    password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
    bindSystem: [{ required: true, message: '请选择绑定系统', trigger: 'change' }]
}

const loadMineTree = async () => {
    try {
        const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 })
        const mines = res.data.result || []
        state.mineList = mines
        state.treeData = mines.map((m: any) => ({ id: m.id, label: m.name }))
    } catch (error) {
        console.error('加载煤矿列表失败:', error)
    }
}

const handleNodeClick = (data: any) => {
    state.queryParams.mineId = data.id
    getList()
}

const getList = async () => {
    state.loading = true
    try {
        const res = await getAPI(FtpConfigApi).getPage(state.queryParams)
        state.tableData = res.data.result?.rows || res.data.result || []
        state.total = res.data.result?.total || 0
    } catch (error) {
        console.error('获取FTP配置列表失败:', error)
    } finally {
        state.loading = false
    }
}

const handleQuery = () => {
    state.queryParams.page = 1
    getList()
}

const resetQuery = () => {
    state.queryParams.mineId = null
    state.queryParams.username = ''
    state.queryParams.bindSystem = ''
    handleQuery()
}

const openAdd = () => {
    state.form = {
        mineId: state.queryParams.mineId,
        host: '',
        port: 21,
        username: '',
        password: '',
        rootDirectory: '/',
        bindSystem: '',
        enabled: 1,
        remark: ''
    }
    state.isEdit = false
    state.dialogVisible = true
}

const openEdit = (row: any) => {
    state.form = { ...row, password: '' }
    state.isEdit = true
    state.dialogVisible = true
}

const delData = async (row: any) => {
    try {
        await ElMessageBox.confirm('确认删除该FTP配置吗？', '提示', { type: 'warning' })
        await getAPI(FtpConfigApi).delete(row.id)
        ElMessage.success('删除成功')
        getList()
    } catch (error) {
        console.error('删除失败:', error)
    }
}

const submitForm = async () => {
    state.submitLoading = true
    try {
        if (state.isEdit) {
            await getAPI(FtpConfigApi).update(state.form)
        } else {
            await getAPI(FtpConfigApi).add(state.form)
        }
        ElMessage.success(state.isEdit ? '编辑成功' : '新增成功')
        state.dialogVisible = false
        getList()
    } catch (error) {
        console.error('操作失败:', error)
    } finally {
        state.submitLoading = false
    }
}

onMounted(async () => {
    await loadMineTree()
    getList()
})
</script>

<style scoped>
.ftp-layout {
    display: flex;
    height: 100%;
    padding: 16px;
}
.left-tree {
    width: 250px;
    margin-right: 16px;
}
.right-content {
    flex: 1;
}
.search-form {
    margin-bottom: 16px;
}
</style>
