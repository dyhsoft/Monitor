<template>
    <div class="tree-layout">
        <div class="left-tree">
            <el-card shadow="hover">
                <template #header>
                    <div style="display: flex; justify-content: space-between; align-items: center;">
                        <span style="font-weight: bold;">选择煤矿</span>
                        <el-button type="primary" text @click="loadTree">刷新</el-button>
                    </div>
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
            <router-view />
        </div>
    </div>
</template>

<script setup lang="ts">
import { reactive, provide, onMounted } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi } from '/@/api-services/api';

const state = reactive({
    treeData: [] as any[],
    treeProps: {
        children: 'children',
        label: 'name'
    },
    selectedMineId: null as number | null,
    selectedMineName: ''
});

provide('selectedMine', { mineId: state.selectedMineId, mineName: state.selectedMineName });

function loadTree() {
    getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 }).then((res) => {
        state.treeData = (res.data.result || []).map((item: any) => ({
            id: item.id,
            name: item.name,
            children: []
        }));
    });
}

function handleNodeClick(data: any) {
    state.selectedMineId = data.id;
    state.selectedMineName = data.name;
    provide('selectedMine', { mineId: data.id, mineName: data.name });
}

onMounted(() => {
    loadTree();
});

defineExpose({ loadTree });
</script>

<style scoped>
.tree-layout {
    display: flex;
    gap: 10px;
    height: calc(100vh - 110px);
}
.left-tree {
    width: 250px;
    overflow: auto;
}
.right-content {
    flex: 1;
    overflow: auto;
}
</style>
