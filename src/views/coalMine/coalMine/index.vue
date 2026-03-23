<template>
    <div class="page-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" :inline="true">
                <el-form-item label="煤矿编码">
                    <el-input v-model="state.queryParams.code" placeholder="煤矿编码" clearable />
                </el-form-item>
                <el-form-item label="煤矿名称">
                    <el-input v-model="state.queryParams.name" placeholder="煤矿名称" clearable />
                </el-form-item>
                <el-form-item>
                    <el-button-group>
                        <el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
                        <el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
                    </el-button-group>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" icon="ele-Plus" @click="openAdd"> 新增 </el-button>
                    <el-button type="success" icon="ele-Upload" @click="autoImport"> 自动导入 </el-button>
                </el-form-item>
            </el-form>
        </el-card>

        <el-card class="full-table" shadow="hover" style="margin-top: 10px">
            <el-table :data="state.tableData" v-loading="state.loading" border stripe>
                <el-table-column type="index" label="序号" width="60" align="center" />
                <el-table-column prop="code" label="煤矿编码" min-width="120" align="center" />
                <el-table-column prop="name" label="煤矿名称" min-width="150" align="center" />
                <el-table-column prop="address" label="地址" min-width="180" align="center" show-overflow-tooltip />
                <el-table-column prop="ftpPath" label="FTP根目录" min-width="150" align="center" show-overflow-tooltip />
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

        <EditCoalMine ref="editRef" @refresh="handleQuery" />
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref, inject } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';
import EditCoalMine from './form.vue';

const editRef = ref();
const selectedMine: any = inject('selectedMine', { mineId: null, mineName: '' });

const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        code: '',
        name: ''
    }
});

onMounted(() => {
    handleQuery();
});

function handleQuery() {
    state.loading = true;
    getAPI(CoalMineApi).getPage(state.queryParams).then((res) => {
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
    editRef.value.open();
}

function openEdit(row: any) {
    editRef.value.open(row);
}

function delData(row: any) {
    ElMessageBox.confirm('确定删除该煤矿吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        getAPI(CoalMineApi).delete(row.id).then(() => {
            ElMessage.success('删除成功');
            handleQuery();
        });
    });
}

function autoImport() {
    ElMessageBox.confirm('将从数据文件夹自动扫描并导入煤矿，确定执行吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info'
    }).then(() => {
        getAPI(CoalMineApi).autoImportCoalMines([]).then((res) => {
            ElMessage.success(`导入成功，共导入 ${res.data} 个煤矿`);
            handleQuery();
        });
    });
}
</script>

<style scoped>
.page-container { padding: 10px; }
.full-table { overflow: auto; }
</style>
