<template>
    <div class="listener-config-container">
        <el-card shadow="hover">
            <el-form :model="state.queryParams" ref="queryForm" :inline="true">
                <el-form-item label="煤矿">
                    <el-select v-model="state.queryParams.mineId" placeholder="请选择煤矿" clearable filterable>
                        <el-option v-for="item in state.mineList" :key="item.id" :label="item.name" :value="item.id" />
                    </el-select>
                </el-form-item>
                <el-form-item label="数据类型">
                    <el-select v-model="state.queryParams.dataType" placeholder="请选择" clearable>
                        <el-option label="CDSS(传感器数据)" value="CDSS" />
                        <el-option label="RWSS(人员定位)" value="RWSS" />
                        <el-option label="KYGL(矿压监测)" value="KYGL" />
                        <el-option label="SWJC(水文监测)" value="SWJC" />
                    </el-select>
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
                <el-table-column prop="listenPath" label="监听路径" min-width="200" align="center" show-overflow-tooltip />
                <el-table-column prop="dataType" label="数据类型" width="120" align="center" />
                <el-table-column prop="bindSystem" label="绑定系统" width="100" align="center" />
                <el-table-column prop="filePattern" label="文件匹配" width="100" align="center" />
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

        <EditListenerConfig ref="editRef" @refresh="handleQuery" :mineList="state.mineList" />
    </div>
</template>

<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, ListenerConfigApi } from '/@/api-services/api';
import EditListenerConfig from './form.vue';

const editRef = ref();
const state = reactive({
    loading: false,
    total: 0,
    tableData: [] as any[],
    mineList: [] as any[],
    queryParams: {
        page: 1,
        pageSize: 10,
        mineId: null as number | null,
        dataType: '',
        bindSystem: ''
    }
});

onMounted(() => {
    loadMineList();
    handleQuery();
});

function loadMineList() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.mineList = res.data.result || [];
    });
}

function handleQuery() {
    state.loading = true;
    getAPI(ListenerConfigApi).getPage(state.queryParams).then((res) => {
        state.tableData = res.data.result?.items || [];
        state.total = res.data.result?.total || 0;
    }).finally(() => {
        state.loading = false;
    });
}

function resetQuery() {
    state.queryParams.mineId = null;
    state.queryParams.dataType = '';
    state.queryParams.bindSystem = '';
    handleQuery();
}

function openAdd() {
    editRef.value.open();
}

function openEdit(row: any) {
    editRef.value.open(row);
}

function delData(row: any) {
    ElMessageBox.confirm('确定删除该监听配置吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(() => {
        getAPI(ListenerConfigApi).delete(row.id).then(() => {
            ElMessage.success('删除成功');
            handleQuery();
        });
    });
}
</script>

<style scoped>
.listener-config-container {
    padding: 10px;
}
.full-table {
    height: calc(100vh - 220px);
    overflow: auto;
}
</style>
