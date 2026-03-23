<template>
    <div class="dataSource-layout">
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
            <el-form :inline="true" :model="state.queryForm" class="search-form">
                <el-form-item label="监听类型">
                    <el-select v-model="state.queryForm.listenerType" placeholder="请选择监听类型" clearable>
                        <el-option label="本地目录" :value="1" />
                        <el-option label="FTP" :value="2" />
                    </el-select>
                </el-form-item>
                <el-form-item label="系统类型">
                    <el-select v-model="state.queryForm.bindSystem" placeholder="请选择系统类型" clearable>
                        <el-option label="安全监测" value="安全监测" />
                        <el-option label="人员定位" value="人员定位" />
                        <el-option label="矿压监测" value="矿压监测" />
                        <el-option label="水文监测" value="水文监测" />
                    </el-select>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="getDataList">查询</el-button>
                    <el-button @click="resetQueryForm">重置</el-button>
                    <el-button type="primary" @click="addOrUpdateHandle()">新增</el-button>
                </el-form-item>
            </el-form>

            <el-table v-loading="state.dataListLoading" :data="state.dataList" border stripe>
                <el-table-column type="index" label="序号" width="60" align="center" />
                <el-table-column prop="mineName" label="煤矿" min-width="120" show-overflow-tooltip />
                <el-table-column prop="listenerType" label="监听类型" width="100" align="center">
                    <template #default="{ row }">
                        <el-tag :type="row.listenerType === 1 ? 'success' : 'warning'">
                            {{ row.listenerType === 1 ? '本地目录' : 'FTP' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="listenPath" label="监听路径" min-width="180" show-overflow-tooltip />
                <el-table-column prop="bindSystem" label="系统类型" width="100" align="center" />
                <el-table-column prop="dataType" label="数据类型" width="100" align="center" />
                <el-table-column prop="filePattern" label="文件规则" width="100" align="center" />
                <el-table-column prop="enabled" label="启用" width="80" align="center">
                    <template #default="{ row }">
                        <el-tag :type="row.enabled === 1 ? 'success' : 'info'">
                            {{ row.enabled === 1 ? '是' : '否' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="180" align="center" fixed="right">
                    <template #default="{ row }">
                        <el-button type="primary" link @click="addOrUpdateHandle(row)">编辑</el-button>
                        <el-button type="danger" link @click="delData(row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>

            <el-pagination
                v-model:current-page="state.queryForm.page"
                v-model:page-size="state.queryForm.pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="getDataList"
                @current-change="getDataList"
            />

            <el-dialog v-model="state.dialogVisible" :title="state.isEdit ? '编辑数据源' : '新增数据源'" width="700px">
                <el-form ref="formRef" :model="state.form" :rules="rules" label-width="100px">
                    <el-form-item label="煤矿" prop="mineId">
                        <el-select v-model="state.form.mineId" placeholder="请选择煤矿">
                            <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="监听类型" prop="listenerType">
                        <el-radio-group v-model="state.form.listenerType" @change="handleListenerTypeChange">
                            <el-radio :value="1">本地目录</el-radio>
                            <el-radio :value="2">FTP</el-radio>
                        </el-radio-group>
                    </el-form-item>
                    
                    <!-- 本地目录配置 -->
                    <template v-if="state.form.listenerType === 1">
                        <el-form-item label="监听路径" prop="listenPath">
                            <el-input v-model="state.form.listenPath" placeholder="如: D:\FTPData\Mine001" />
                        </el-form-item>
                        <el-form-item label="处理后目录" prop="processedPath">
                            <el-input v-model="state.form.processedPath" placeholder="处理完成后文件移动到的目录（空则删除）" />
                        </el-form-item>
                    </template>
                    
                    <!-- FTP配置 -->
                    <template v-if="state.form.listenerType === 2">
                        <el-form-item label="FTP地址" prop="host">
                            <el-input v-model="state.form.host" placeholder="如: 192.168.1.100" />
                        </el-form-item>
                        <el-form-item label="FTP端口" prop="port">
                            <el-input-number v-model="state.form.port" :min="1" :max="65535" />
                        </el-form-item>
                        <el-form-item label="用户名" prop="username">
                            <el-input v-model="state.form.username" placeholder="FTP用户名" />
                        </el-form-item>
                        <el-form-item label="密码" prop="password">
                            <el-input v-model="state.form.password" type="password" placeholder="FTP密码" show-password />
                        </el-form-item>
                        <el-form-item label="FTP目录" prop="listenPath">
                            <el-input v-model="state.form.listenPath" placeholder="如: /data" />
                        </el-form-item>
                    </template>
                    
                    <el-form-item label="系统类型" prop="bindSystem">
                        <el-select v-model="state.form.bindSystem" placeholder="请选择系统类型">
                            <el-option label="安全监测" value="安全监测" />
                            <el-option label="人员定位" value="人员定位" />
                            <el-option label="矿压监测" value="矿压监测" />
                            <el-option label="水文监测" value="水文监测" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="数据类型" prop="dataType">
                        <el-select v-model="state.form.dataType" placeholder="请选择数据类型">
                            <el-option label="CDSS-安全监测" value="CDSS" />
                            <el-option label="CDDY-报警数据" value="CDDY" />
                            <el-option label="FZSS-负压数据" value="FZSS" />
                            <el-option label="RYSS-人员定位" value="RYSS" />
                            <el-option label="RYCS-人员初始化" value="RYCS" />
                            <el-option label="RYCY-人员出勤" value="RYCY" />
                            <el-option label="RYQJ-区域人数" value="RYQJ" />
                            <el-option label="JZSS-基站状态" value="JZSS" />
                            <el-option label="CGKCDSS-水害监测" value="CGKCDSS" />
                            <el-option label="KYCDSS-矿压监测" value="KYCDSS" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="文件规则" prop="filePattern">
                        <el-input v-model="state.form.filePattern" placeholder="如: *.txt" />
                    </el-form-item>
                    <el-form-item label="字段分隔符">
                        <el-input v-model="state.form.fieldSeparator" placeholder="默认 ;" style="width: 100px;" />
                        <span style="color: #999; margin-left: 8px; font-size: 12px;">分隔数据字段</span>
                    </el-form-item>
                    <el-form-item label="记录分隔符">
                        <el-input v-model="state.form.recordSeparator" placeholder="默认 ~" style="width: 100px;" />
                        <span style="color: #999; margin-left: 8px; font-size: 12px;">分隔每条数据记录</span>
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
import { CoalMineApi, ListenerConfigApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'label' },
    queryForm: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        listenerType: null as number | null,
        bindSystem: ''
    },
    dataList: [] as any[],
    dataListLoading: false,
    total: 0,
    mineList: [] as any[],
    dialogVisible: false,
    isEdit: false,
    submitLoading: false,
    form: {} as any
})

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    listenerType: [{ required: true, message: '请选择监听类型', trigger: 'change' }],
    listenPath: [{ required: true, message: '请输入监听路径', trigger: 'blur' }],
    bindSystem: [{ required: true, message: '请选择系统类型', trigger: 'change' }],
    dataType: [{ required: true, message: '请选择数据类型', trigger: 'change' }]
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
    state.queryForm.mineId = data.id
    getDataList()
}

const getDataList = async () => {
    state.dataListLoading = true
    try {
        const res = await getAPI(ListenerConfigApi).getPage(state.queryForm)
        const rows = res.data.result?.rows || res.data.result || []
        // 关联煤矿名称
        state.dataList = rows.map((item: any) => {
            const mine = state.mineList.find(m => m.id === item.mineId)
            return { ...item, mineName: mine?.name || '-' }
        })
        state.total = res.data.result?.total || 0
    } catch (error) {
        console.error('获取数据源列表失败:', error)
    } finally {
        state.dataListLoading = false
    }
}

const resetQueryForm = () => {
    state.queryForm.mineId = null
    state.queryForm.listenerType = null
    state.queryForm.bindSystem = ''
    getDataList()
}

const addOrUpdateHandle = (row?: any) => {
    state.dialogVisible = true
    state.isEdit = !!row
    if (row) {
        state.form = { ...row }
    } else {
        state.form = {
            mineId: state.queryForm.mineId,
            listenerType: 1,
            listenPath: '',
            processedPath: '',
            host: '',
            port: 21,
            username: '',
            password: '',
            bindSystem: '',
            dataType: '',
            filePattern: '*.txt',
            fieldSeparator: ';',
            recordSeparator: '~',
            enabled: 1,
            remark: ''
        }
    }
}

const handleListenerTypeChange = () => {
    state.form.listenPath = ''
    state.form.host = ''
    state.form.port = 21
    state.form.username = ''
    state.form.password = ''
}

const submitForm = async () => {
    const formRef = document.querySelector('.el-dialog form')
    if (!formRef) return
    
    try {
        await (formRef as any).validate()
        state.submitLoading = true
        
        if (state.isEdit) {
            await getAPI(ListenerConfigApi).update(state.form)
            ElMessage.success('编辑成功')
        } else {
            await getAPI(ListenerConfigApi).add(state.form)
            ElMessage.success('新增成功')
        }
        
        state.dialogVisible = false
        getDataList()
    } catch (error: any) {
        console.error('提交失败:', error)
        if (error?.message !== 'cancel') {
            ElMessage.error(error?.message || '操作失败')
        }
    } finally {
        state.submitLoading = false
    }
}

const delData = async (row: any) => {
    try {
        await ElMessageBox.confirm('确定要删除该数据源吗？', '提示', { type: 'warning' })
        await getAPI(ListenerConfigApi).delete(row.id)
        ElMessage.success('删除成功')
        getDataList()
    } catch (error: any) {
        console.error('删除失败:', error)
        if (error !== 'cancel') {
            ElMessage.error('删除失败')
        }
    }
}

onMounted(async () => {
    await loadMineTree()
    getDataList()
})
</script>

<style scoped>
.dataSource-layout {
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
