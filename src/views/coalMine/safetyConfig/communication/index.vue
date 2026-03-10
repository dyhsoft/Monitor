<template>
    <div class="page-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header><span style="font-weight: bold;">选择煤矿</span></template>
                <el-tree :data="state.treeData" :props="state.treeProps" @node-click="handleNodeClick" node-key="id" default-expand-all highlight-current />
            </el-card>
        </div>
        <div class="right-content">
            <el-card shadow="hover">
                <el-form :model="state.form" label-width="120px">
                    <el-form-item label="通信方式">
                        <el-select v-model="state.form.commType" style="width: 200px;">
                            <el-option label="RS485" value="RS485" />
                            <el-option label="TCP/IP" value="TCP" />
                            <el-option label="UDP" value="UDP" />
                            <el-option label="无线" value="Wireless" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="采集器IP"><el-input v-model="state.form.collectorIp" style="width: 200px;" /></el-form-item>
                    <el-form-item label="端口"><el-input-number v-model="state.form.port" :min="1" :max="65535" /></el-form-item>
                    <el-form-item label="波特率">
                        <el-select v-model="state.form.baudRate" style="width: 200px;">
                            <el-option label="9600" value="9600" />
                            <el-option label="19200" value="19200" />
                            <el-option label="38400" value="38400" />
                            <el-option label="115200" value="115200" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="轮询间隔(秒)"><el-input-number v-model="state.form.pollInterval" :min="1" /></el-form-item>
                    <el-form-item><el-button type="primary" @click="save">保存配置</el-button></el-form-item>
                </el-form>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const state = reactive({
    treeData: [] as any[], treeProps: { children: 'children', label: 'name' },
    queryParams: { mineId: null as number | null },
    form: { commType: 'TCP', collectorIp: '192.168.1.100', port: 5001, baudRate: '9600', pollInterval: 10 }
});

onMounted(() => { loadMineTree(); });

function loadMineTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({ id: item.id, name: item.name, children: [] }));
    });
}

function handleNodeClick(data: any) { state.queryParams.mineId = data.id; }
function save() { if (!state.queryParams.mineId) { ElMessage.warning('请先选择煤矿'); return; } ElMessage.success('保存成功'); }
</script>

<style scoped>
.page-layout { display: flex; gap: 10px; height: calc(100vh - 150px); }
.left-tree { width: 250px; overflow: auto; }
.right-content { flex: 1; overflow: auto; }
</style>
