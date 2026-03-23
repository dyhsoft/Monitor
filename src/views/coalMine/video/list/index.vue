<template>
  <div class="video-container">
    <!-- 左侧面板 -->
    <div class="left-panel">
      <!-- 煤矿选择 -->
      <div class="mine-select">
        <el-select v-model="selectedMine" placeholder="请选择煤矿" @change="handleMineChange">
          <el-option v-for="item in mineOptions" :key="item.value" :label="item.label" :value="item.value"></el-option>
        </el-select>
      </div>
      
      <!-- 摄像头树形结构 -->
      <div class="camera-tree">
        <el-tree
          :data="treeData"
          :props="treeProps"
          @node-click="handleNodeClick"
          default-expand-all
        >
          <template #default="{ node, data }">
            <span class="custom-tree-node">
              <span class="camera-icon">
                <el-icon v-if="data.type === 'area'"><Location /></el-icon>
                <el-icon v-else><VideoCamera /></el-icon>
              </span>
              <span>{{ node.label }}</span>
              <span class="camera-status" :class="data.status">{{ data.status === 'online' ? '在线' : '离线' }}</span>
            </span>
          </template>
        </el-tree>
      </div>
    </div>
    
    <!-- 右侧视频区域 -->
    <div class="right-panel">
      <div v-if="currentVideo" class="video-player">
        <div class="video-cover">
          <div class="video-placeholder">
            <el-icon :size="60"><VideoCamera /></el-icon>
            <p>{{ currentVideo.name }}</p>
          </div>
        </div>
        <div class="video-controls">
          <el-button type="primary" size="small">播放</el-button>
          <el-button size="small">截图</el-button>
          <el-button size="small">云台控制</el-button>
        </div>
        <div class="video-info">
          <span>状态: {{ currentVideo.status === 'online' ? '在线' : '离线' }}</span>
          <span>通道: {{ currentVideo.channel }}</span>
        </div>
      </div>
      <div v-else class="no-video">
        <el-empty description="请选择摄像头" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Location, VideoCamera } from '@element-plus/icons-vue'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, VideoApi } from '/@/api-services/api';
import { ElMessage } from 'element-plus';

const selectedMine = ref<number | null>(null)
const currentVideo = ref<any>(null)
const mineOptions = ref<any[]>([])
const treeData = ref<any[]>([])

const treeProps = { children: 'children', label: 'label' }

// 加载煤矿列表
const loadMineOptions = async () => {
  try {
    const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 100 });
    mineOptions.value = (res.data.result || []).map((item: any) => ({ label: item.name, value: item.id }));
    if (mineOptions.value.length > 0) {
      selectedMine.value = mineOptions.value[0].value;
      loadVideoTree();
    }
  } catch (error) {
    console.error('加载煤矿列表失败:', error);
  }
}

// 加载视频树
const loadVideoTree = async () => {
  if (!selectedMine.value) return;
  try {
    const res = await getAPI(VideoApi).getSelectList(selectedMine.value);
    const videos = res.data.result || [];
    
    // 按区域分组
    const areaMap = new Map<string, any[]>();
    videos.forEach((v: any) => {
      const area = v.location || '未分组';
      if (!areaMap.has(area)) {
        areaMap.set(area, []);
      }
      areaMap.get(area)!.push({
        label: v.cameraName,
        type: 'camera',
        status: 'online',
        channel: v.cameraCode,
        id: v.id,
        streamUrl: v.streamUrl
      });
    });
    
    treeData.value = Array.from(areaMap.entries()).map(([area, children]) => ({
      label: area,
      type: 'area',
      children
    }));
  } catch (error) {
    console.error('加载视频列表失败:', error);
    treeData.value = [];
  }
}

const handleMineChange = () => {
  currentVideo.value = null;
  loadVideoTree();
}

const handleNodeClick = (data: any) => {
  if (data.type === 'camera') {
    currentVideo.value = data;
  }
}

onMounted(() => {
  loadMineOptions();
})
</script>

<style scoped>
.video-container {
  display: flex;
  height: calc(100vh - 140px);
  padding: 16px;
  gap: 16px;
}

.left-panel {
  width: 320px;
  background: #fff;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
}

.mine-select {
  padding: 16px;
  border-bottom: 1px solid #eee;
}

.mine-select .el-select {
  width: 100%;
}

.camera-tree {
  flex: 1;
  padding: 16px;
  overflow: auto;
}

.custom-tree-node {
  display: flex;
  align-items: center;
  width: 100%;
}

.camera-icon {
  margin-right: 8px;
  display: flex;
  align-items: center;
}

.camera-status {
  margin-left: auto;
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 4px;
}

.camera-status.online {
  background: #67c23a;
  color: #fff;
}

.camera-status.offline {
  background: #909399;
  color: #fff;
}

.right-panel {
  flex: 1;
  background: #fff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.video-player {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.video-cover {
  flex: 1;
  background: #000;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px 8px 0 0;
}

.video-placeholder {
  text-align: center;
  color: #666;
}

.video-placeholder p {
  margin-top: 16px;
  color: #fff;
}

.video-controls {
  padding: 16px;
  display: flex;
  gap: 8px;
  justify-content: center;
  background: #f5f5f5;
}

.video-info {
  padding: 12px 16px;
  background: #f5f5f5;
  display: flex;
  gap: 24px;
  font-size: 14px;
  color: #666;
  border-radius: 0 0 8px 8px;
}

.no-video {
  text-align: center;
}
</style>
