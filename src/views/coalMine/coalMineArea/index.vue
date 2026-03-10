<template>
    <div class="page-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" :inline="true">
                <el-form-item label="区域编码">
                    <el-input v-model="state.queryParams.code" placeholder="区域编码" clearable />
                </el-form-item>
                <el-form-item label="区域名称">
                    <el-input v-model="state.queryParams.name" placeholder="区域名称" clearable />
                </el-form-item>
                <el-form-item>
                    <el-button-group>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
                        <el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
                    </el-button-group>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Plus" @click="openAdd"> 新增 </el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 10px">
            <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                <el-table-column type="index" label="序号" width="60" align="center" />
                <el-table-column prop="mineName" label="所属煤矿" min-width="120" align="center" />
                <el-table-column prop="code" label="区域编码" min-width="120" align="center" />
                <el-table-column prop="name" label="区域名称" min-width="150" align="center" />
                <el-table-column label="区域类型" width="100" align="center">
                    <template #default="scope">
                        <el-tag v-if="scope.row.type === 1">大巷</el-tag>
                        <el-tag v-else-if="scope.row.type === 2">工作面</el-tag>
                        <el-tag v-else-if="scope.row.type === 3">硐室</el-tag>
                    </template>
                </el-table-column>
                <el-table-column prop="x" label="坐标X" width="120" align="center" />
                <el-table-column prop="y" label="坐标Y" width="120" align="center" />
                <el-table-column prop="capacity" label="容纳人数" width="90" align="center" />
                <el-table-column label="状态" width="80" align="center">
                    <template #default="scope">
                        <el-tag :type="scope.row.enabled === 1 ? 'success' : 'danger'">
                            {{ scope.row.enabled === 1 ? '启用' : '禁用' }}
                        </el-tag>
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="180" align="center" fixed="right">
                    <template #default="scope">
                        <el-button icon="ele-Edit" text type="primary" @click="openEdit(scope.row)"> 编辑 </el-button>
                        <el-button icon="ele-Delete" text type="danger" @click="delData(scope.row)"> 删除 </el-button>
                    </template>
                </el-table-column>
            </el-table>
            <el-pagination 
                v-model:current-page="state.queryParams.page" 
                v-model:page-size="state.queryParams.pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :total="state.total"
                layout="total, sizes, prev, pager, next, jumper"
                @size-change="handleQuery"
                @current-change="handleQuery"
                style="margin-top: 10px" 
            />
        </el-card>

        <EditCoalMineArea ref="editRef" @refresh="handleQuery" />
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref, inject } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, CoalMineAreaApi } from '/@/api-services/api';
import EditCoalMineArea from './form.vue';

const editRef = ref();
const selectedMine: any = inject('selectedMine', { mineId: null, mineName: '' });

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        code: '',
        name: ''
    }
});

onMounted(() => {
    state.queryParams.mineId = selectedMine.mineId;
    handleQuery();
});

function handleQuery() {
    state.loading = true;
    getAPI(CoalMineAreaApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.code = '';
    state.queryParams.name = '';
    handleQuery();
}

function openAdd() {
    if (!state.queryParams.mineId) {
        ElMessage.warning('请先在左侧选择煤矿');
        return;
    }
    editRef.value.open(state.queryParams.mineId);
}

function openEdit(row: any) {
    editRef.value.open(row);
}

function delData(row: any) {
    ElMessageBox.confirm('确定删除该区域吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        getAPI(CoalMineAreaApi).delete(row.id).then(() => {
            ElMessage.success('删除成功');
            handleQuery();
        });
    });
}
</script>

<style scoped>
.page-container { padding: 10px; }
.full-table { overflow: auto; }
</style>
