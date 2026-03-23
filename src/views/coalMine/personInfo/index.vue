<template>
    <div class="person-layout">
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
                <el-form-item label="姓名">
                    <el-input v-model="queryParams.name" placeholder="请输入姓名" clearable @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item label="工号">
                    <el-input v-model="queryParams.jobNum" placeholder="请输入工号" clearable @keyup.enter="handleQuery" />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleQuery">查询</el-button>
                    <el-button @click="resetQuery">重置</el-button>
                    <el-button type="primary" @click="handleAdd">新增</el-button>
                </el-form-item>
            </el-form>

            <el-table v-loading="loading" :data="personList" border stripe>
                <el-table-column label="序号" type="index" width="60" align="center" />
                <el-table-column label="煤矿" prop="mineName" align="center" />
                <el-table-column label="姓名" prop="name" align="center" />
                <el-table-column label="工号" prop="jobNum" align="center" />
                <el-table-column label="部门" prop="department" align="center" />
                <el-table-column label="职位" prop="position" align="center" />
                <el-table-column label="电话" prop="phone" align="center" />
                <el-table-column label="状态" prop="status" align="center">
                    <template #default="{ row }">
                        <el-tag :type="row.status === 1 ? 'success' : 'danger'">
                            {{ row.status === 1 ? '在岗' : '离岗' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="180" align="center">
                    <template #default="{ row }">
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

            <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑人员' : '新增人员'" width="600px">
                <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
                    <el-form-item label="煤矿" prop="mineId">
                        <el-select v-model="form.mineId" placeholder="请选择煤矿">
                            <el-option v-for="item in mineList" :key="item.value" :label="item.label" :value="item.value"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="姓名" prop="name">
                        <el-input v-model="form.name" placeholder="请输入姓名" />
                    </el-form-item>
                    <el-form-item label="工号" prop="jobNum">
                        <el-input v-model="form.jobNum" placeholder="请输入工号" />
                    </el-form-item>
                    <el-form-item label="部门" prop="department">
                        <el-input v-model="form.department" placeholder="请输入部门" />
                    </el-form-item>
                    <el-form-item label="职位" prop="position">
                        <el-input v-model="form.position" placeholder="请输入职位" />
                    </el-form-item>
                    <el-form-item label="电话" prop="phone">
                        <el-input v-model="form.phone" placeholder="请输入电话" />
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
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonApi } from '/@/api-services/api';

const loading = ref(false)
const total = ref(0)
const mineList = ref<any[]>([])
const personList = ref<any[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitLoading = ref(false)
const formRef = ref()

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'label' }
})

const queryParams = reactive({
    page: 1,
    pageSize: 10,
    mineId: null as number | null,
    name: '',
    jobNum: ''
})

const form = reactive<any>({
    mineId: null,
    name: '',
    jobNum: '',
    department: '',
    position: '',
    phone: '',
    remark: ''
})

const rules = {
    mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
    name: [{ required: true, message: '请输入姓名', trigger: 'blur' }],
    jobNum: [{ required: true, message: '请输入工号', trigger: 'blur' }]
}

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

const handleNodeClick = (data: any) => {
    queryParams.mineId = data.id
    getList()
}

const getList = async () => {
    loading.value = true
    try {
        const res = await getAPI(PersonApi).getPage(queryParams)
        personList.value = res.data.result?.rows || res.data.result || []
        total.value = res.data.result?.total || 0
    } catch (error) {
        console.error('获取人员列表失败:', error)
    } finally {
        loading.value = false
    }
}

const handleQuery = () => {
    queryParams.page = 1
    getList()
}

const resetQuery = () => {
    queryParams.mineId = null
    queryParams.name = ''
    queryParams.jobNum = ''
    handleQuery()
}

const handleAdd = () => {
    Object.assign(form, { mineId: queryParams.mineId, name: '', jobNum: '', department: '', position: '', phone: '', remark: '' })
    isEdit.value = false
    dialogVisible.value = true
}

const handleEdit = (row: any) => {
    Object.assign(form, row)
    isEdit.value = true
    dialogVisible.value = true
}

const handleDelete = async (row: any) => {
    try {
        await ElMessageBox.confirm('确认删除该人员吗？', '提示', { type: 'warning' })
        await getAPI(PersonApi).delete(row.id)
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
        if (isEdit.value) {
            await getAPI(PersonApi).update(form)
        } else {
            await getAPI(PersonApi).add(form)
        }
        ElMessage.success(isEdit.value ? '编辑成功' : '新增成功')
        dialogVisible.value = false
        getList()
    } catch (error) {
        console.error('操作失败:', error)
    } finally {
        submitLoading.value = false
    }
}

onMounted(async () => {
    await loadMineTree()
    getList()
})
</script>

<style scoped>
.person-layout {
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
