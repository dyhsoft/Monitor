<template>
    <div class="video-config-layout">
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
            <el-form :inline="true" :model="queryParams" class="search-form">
                <el-form-item label="配置名称">
                    <el-input v-model="queryParams.configName" placeholder="请输入配置名称" clearable @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleQuery">查询</el-button>
                    <el-button @click="resetQuery">重置</el-button>
                    <el-button type="primary" @click="handleAdd">新增</el-button>
                </el-form-item>
            </el-form>

            <el-table v-loading="loading" :data="configList" border stripe>
                <el-table-column label="序号" type="index" width="60" align="center" />
                <el-table-column label="煤矿" prop="mineName" align="center" />
                <el-table-column label="配置名称" prop="configName" align="center" />
                <el-table-column label="API地址" prop="apiUrl" align="center" min-width="200" />
                <el-table-column label="AppKey" prop="appKey" align="center" min-width="150" />
                <el-table-column label="状态" prop="status" align="center">
                    <template #default="{ row }">
                        <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                            {{ row.status === 1 ? '在线' : '离线' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="创建时间" prop="createTime" align="center" width="180" />
                <el-table-column label="操作" width="240" align="center">
                    <template #default="{ row }">
                        <el-button type="primary" link @click="handleTest(row)">测试</el-button>
                        <el-button type="primary" link @click="handleGetArea(row)">获取区域</el-button>
                        <el-button type="primary" link @click="handleGetChannel(row)">获取通道</el-button>
                        <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
                        <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>

            <el-pagination
                v-model:current-page="queryParams.page"
                v-model:page-size="queryParams.pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="getList"
                @current-change="getList"
            />

            <!-- 新增/编辑对话框 -->
            <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑配置' : '新增配置'" width="600px">
                <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
                    <el-form-item label="配置名称" prop="configName">
                        <el-input v-model="form.configName" placeholder="请输入配置名称" />
                    </el-form-item>
                    <el-form-item label="煤矿" prop="mineId">
                        <el-select v-model="form.mineId" placeholder="请选择煤矿">
                            <el-option v-for="item in mineList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="API地址" prop="apiUrl">
                        <el-input v-model="form.apiUrl" placeholder="例如: https://127.0.0.1:8443" />
                    </el-form-item>
                    <el-form-item label="AppKey" prop="appKey">
                        <el-input v-model="form.appKey" placeholder="请输入AppKey" />
                    </el-form-item>
                    <el-form-item label="AppSecret" prop="appSecret">
                        <el-input v-model="form.appSecret" type="password" placeholder="请输入AppSecret" show-password />
                    </el-form-item>
                    <el-form-item label="备注" prop="remark">
                        <el-input v-model="form.remark" type="textarea" :rows="2" />
                    </el-form-item>
                </el-form>
                <template #footer>
                    <el-button @click="dialogVisible = false">取消</el-button>
                    <el-button type="primary" @click="submitForm" :loading="submitLoading">确定</el-button>
                </template>
            </el-dialog>

            <!-- 区域/通道结果对话框 -->
            <el-dialog v-model="resultDialogVisible" :title="resultTitle" width="800px">
                <el-table :data="resultData" border stripe max-height="400">
                    <el-table-column prop="indexCode" label="编号" />
                    <el-table-column prop="name" label="名称" />
                    <el-table-column prop="parentIndexCode" label="父级编号" />
                </el-table>
            </el-dialog>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const loading = ref(false)
const total = ref(0)
const mineList = ref<any[]>([])
const configList = ref<any[]>([])
const dialogVisible = ref(false)
const resultDialogVisible = ref(false)
const isEdit = ref(false)
const submitLoading = ref(false)
const formRef = ref()
const resultTitle = ref('')
const resultData = ref<any[]>([])

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'label' }
})

const queryParams = reactive({
    page: 1,
    pageSize: 10,
    mineId: null as number | null,
    configName: ''
})

const form = reactive<any>({
    id: null,
    configName: '',
    mineId: null,
    apiUrl: '',
    appKey: '',
    appSecret: '',
    remark: ''
})

const rules = {
    configName: [{ required: true, message: '请输入配置名称', trigger: 'blur' }],
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    apiUrl: [{ required: true, message: '请输入API地址', trigger: 'blur' }],
    appKey: [{ required: true, message: '请输入AppKey', trigger: 'blur' }],
    appSecret: [{ required: true, message: '请输入AppSecret', trigger: 'blur' }]
}

// 加载煤矿树
const loadMineTree = async () => {
    try {
        const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 })
        const mines = res.data.result || []
        mineList.value = mines.map((m: any) => ({ label: m.name, value: m.id }))
        state.treeData = mines.map((m: any) => ({ id: m.id, label: m.name }))
    } catch (error) {
        console.error('获取煤矿列表失败:', error)
    }
}

// 加载配置列表
const getList = async () => {
    loading.value = true
    try {
        // TODO: 后端视频平台配置服务完成后替换为真实API
        // 模拟数据
        const mockData = [
            {
                id: 1,
                mineId: queryParams.mineId || 1,
                mineName: '测试煤矿',
                configName: '海康视频配置1',
                apiUrl: 'https://127.0.0.1:8443',
                appKey: 'XXXXXXXXXXXX',
                status: 1,
                createTime: '2026-03-10 10:00:00'
            },
            {
                id: 2,
                mineId: queryParams.mineId || 1,
                mineName: '测试煤矿',
                configName: '海康视频配置2',
                apiUrl: 'https://192.168.1.100:8443',
                appKey: 'YYYYYYYYYYYY',
                status: 0,
                createTime: '2026-03-11 14:30:00'
            }
        ]
        
        // 按煤矿过滤
        let filteredData = mockData
        if (queryParams.mineId) {
            filteredData = mockData.filter((item: any) => item.mineId === queryParams.mineId)
        }
        if (queryParams.configName) {
            filteredData = filteredData.filter((item: any) => item.configName.includes(queryParams.configName))
        }
        
        configList.value = filteredData
        total.value = filteredData.length
        
        // 真实API调用示例（后端服务完成后启用）
        // const res = await getAPI(VideoPlatformApi).getPage(queryParams)
        // configList.value = res.data.result?.rows || res.data.result || []
        // total.value = res.data.result?.total || 0
    } catch (error) {
        console.error('获取配置列表失败:', error)
    } finally {
        loading.value = false
    }
}

const handleNodeClick = (data: any) => {
    queryParams.mineId = data.id
    getList()
}

const handleQuery = () => {
    queryParams.page = 1
    getList()
}

const resetQuery = () => {
    queryParams.mineId = null
    queryParams.configName = ''
    handleQuery()
}

const handleAdd = () => {
    Object.assign(form, { 
        id: null,
        configName: '', 
        mineId: queryParams.mineId, 
        apiUrl: '', 
        appKey: '', 
        appSecret: '',
        remark: '' 
    })
    isEdit.value = false
    dialogVisible.value = true
}

const handleEdit = (row: any) => {
    Object.assign(form, { ...row })
    isEdit.value = true
    dialogVisible.value = true
}

const handleDelete = async (row: any) => {
    try {
        await ElMessageBox.confirm('确认删除该配置吗？', '提示', { type: 'warning' })
        // TODO: 后端服务完成后替换为真实API
        // await getAPI(VideoPlatformApi).delete(row.id)
        ElMessage.success('删除成功')
        getList()
    } catch (error) {
        console.error('删除失败:', error)
    }
}

const submitForm = async () => {
    if (!formRef.value) return
    await formRef.value.validate()
    submitLoading.value = true
    try {
        // TODO: 后端服务完成后替换为真实API
        // if (isEdit.value) {
        //     await getAPI(VideoPlatformApi).update(form)
        // } else {
        //     await getAPI(VideoPlatformApi).add(form)
        // }
        ElMessage.success(isEdit.value ? '编辑成功' : '新增成功')
        dialogVisible.value = false
        getList()
    } catch (error) {
        console.error('操作失败:', error)
    } finally {
        submitLoading.value = false
    }
}

const handleTest = (row: any) => {
    ElMessage.info('正在测试连接...')
    setTimeout(() => {
        ElMessage.success('连接测试成功')
    }, 1000)
}

const handleGetArea = (row: any) => {
    resultTitle.value = '区域列表'
    resultData.value = [
        { indexCode: '1', name: '测试区域1', parentIndexCode: '' },
        { indexCode: '2', name: '测试区域2', parentIndexCode: '1' }
    ]
    resultDialogVisible.value = true
}

const handleGetChannel = (row: any) => {
    resultTitle.value = '通道列表'
    resultData.value = [
        { indexCode: 'channel1', name: '通道1', parentIndexCode: '1' },
        { indexCode: 'channel2', name: '通道2', parentIndexCode: '1' }
    ]
    resultDialogVisible.value = true
}

onMounted(async () => {
    await loadMineTree()
    getList()
})
</script>

<style scoped>
.video-config-layout {
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
