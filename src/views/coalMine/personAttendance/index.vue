<template>
    <div class="attendance-layout">
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
                <el-form-item label="日期">
                    <el-date-picker
                        v-model="queryParams.date"
                        type="date"
                        placeholder="请选择日期"
                        @change="handleQuery"
                    />
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleQuery">查询</el-button>
                    <el-button @click="resetQuery">重置</el-button>
                </el-form-item>
            </el-form>

            <el-table v-loading="loading" :data="attendanceList" border stripe>
                <el-table-column label="序号" type="index" width="60" align="center" />
                <el-table-column label="煤矿" prop="mineName" align="center" />
                <el-table-column label="姓名" prop="name" align="center" />
                <el-table-column label="工号" prop="jobNum" align="center" />
                <el-table-column label="部门" prop="department" align="center" />
                <el-table-column label="签到时间" prop="checkInTime" align="center" width="180" />
                <el-table-column label="签退时间" prop="checkOutTime" align="center" width="180" />
                <el-table-column label="工作时长" prop="workHours" align="center" />
                <el-table-column label="状态" prop="status" align="center">
                    <template #default="{ row }">
                        <el-tag v-if="row.status === '正常'" type="success">正常</el-tag>
                        <el-tag v-else-if="row.status === '迟到'" type="warning">迟到</el-tag>
                        <el-tag v-else-if="row.status === '早退'" type="warning">早退</el-tag>
                        <el-tag v-else type="info">缺勤</el-tag>
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
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, PersonAttendanceApi } from '/@/api-services/api';

const loading = ref(false)
const total = ref(0)
const attendanceList = ref<any[]>([])

const state = reactive({
    treeData: [] as any[],
    treeProps: { children: 'children', label: 'label' }
})

const queryParams = reactive({
    page: 1,
    pageSize: 10,
    mineId: null as number | null,
    name: '',
    date: ''
})

const loadMineTree = async () => {
    try {
        const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 })
        const mines = res.data.result || []
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
        const res = await getAPI(PersonAttendanceApi).getPage(queryParams)
        attendanceList.value = res.data.result?.rows || res.data.result || []
        total.value = res.data.result?.total || 0
    } catch (error) {
        console.error('获取出勤列表失败:', error)
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
    queryParams.date = ''
    handleQuery()
}

onMounted(async () => {
    await loadMineTree()
    getList()
})
</script>

<style scoped>
.attendance-layout {
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
