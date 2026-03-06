<template>
  <div class="dashboard">
    <!-- 顶部标题 -->
    <div class="dashboard-header">
      <h1>煤矿安全监控综合管理平台</h1>
      <div class="header-time">{{ currentTime }}</div>
    </div>

    <!-- 主要内容区 -->
    <div class="dashboard-content">
      <!-- 左侧 -->
      <div class="panel left-panel">
        <!-- 煤矿列表 -->
        <n-card title="煤矿列表" class="panel-card">
          <n-list hoverable clickable>
            <n-list-item v-for="mine in mineList" :key="mine.id" @click="selectMine(mine)">
              <n-thing :title="mine.name" :description="`编号: ${mine.code}`">
                <template #header-extra>
                  <n-tag :type="mine.online ? 'success' : 'default'" size="small">
                    {{ mine.online ? '在线' : '离线' }}
                  </n-tag>
                </template>
              </n-thing>
            </n-list-item>
          </n-list>
        </n-card>

        <!-- 安全监测 -->
        <n-card title="安全监测实时数据" class="panel-card">
          <n-space vertical>
            <n-statistic label="测点总数" :value="safetyStats.total">
              <template #prefix>📊</template>
            </n-statistic>
            <n-statistic label="正常" :value="safetyStats.normal" :value-style="{ color: '#18a058' }">
              <template #prefix>✅</template>
            </n-statistic>
            <n-statistic label="报警" :value="safetyStats.alarm" :value-style="{ color: '#d03050' }">
              <template #prefix>⚠️</template>
            </n-statistic>
            <n-statistic label="故障" :value="safetyStats.fault" :value-style="{ color: '#f0a020' }">
              <template #prefix>❌</template>
            </n-statistic>
          </n-space>
        </n-card>

        <!-- 水害监测 -->
        <n-card title="水害监测" class="panel-card">
          <n-space vertical>
            <n-statistic label="监测点数" :value="waterStats.total">
              <template #prefix>💧</template>
            </n-statistic>
            <n-progress
              type="line"
              :percentage="waterStats.percentage"
              :indicator-placement="'inside'"
              :height="24"
              :color="waterStats.percentage > 80 ? '#d03050' : '#18a058'"
            >
              水位正常率
            </n-progress>
          </n-space>
        </n-card>
      </div>

      <!-- 中间 -->
      <div class="panel center-panel">
        <!-- 地图/示意图 -->
        <n-card title="矿井分布示意图" class="panel-card map-card">
          <div class="mine-map">
            <div class="mine-visual">
              <div class="mine-shaft">主井</div>
              <div class="mine-tunnel tunnel-1">运输巷</div>
              <div class="mine-tunnel tunnel-2">回风巷</div>
              <div class="mine-workface">采煤工作面</div>
              <div class="mine-water-pool">水仓</div>
            </div>
          </div>
        </n-card>

        <!-- 报警信息 -->
        <n-card title="实时报警信息" class="panel-card alarm-card">
          <n-scrollbar style="max-height: 200px">
            <n-list hoverable>
              <n-list-item v-for="alarm in alarmList" :key="alarm.id">
                <n-thing>
                  <template #header>
                    <n-space>
                      <n-tag :type="getAlarmType(alarm.level)" size="small">
                        {{ getAlarmLevelName(alarm.level) }}
                      </n-tag>
                      <span>{{ alarm.sensorName }}</span>
                    </n-space>
                  </template>
                  <template #description>
                    <span class="alarm-desc">{{ alarm.description }}</span>
                    <span class="alarm-time">{{ alarm.alarmTime }}</span>
                  </template>
                </n-thing>
              </n-list-item>
              <n-empty v-if="alarmList.length === 0" description="暂无报警" />
            </n-list>
          </n-scrollbar>
        </n-card>
      </div>

      <!-- 右侧 -->
      <div class="panel right-panel">
        <!-- 人员定位统计 -->
        <n-card title="人员定位统计" class="panel-card">
          <n-space vertical>
            <n-statistic label="井下总人数" :value="personStats.total">
              <template #prefix>👷</template>
            </n-statistic>
            <n-divider />
            <div class="person-distribution">
              <div class="dist-item">
                <span class="dist-label">采煤面</span>
                <n-progress type="line" :percentage="personStats.caimeiPercent" :show-indicator="false" />
                <span class="dist-value">{{ personStats.caimei }}</span>
              </div>
              <div class="dist-item">
                <span class="dist-label">掘进面</span>
                <n-progress type="line" :percentage="personStats.juejinPercent" :show-indicator="false" />
                <span class="dist-value">{{ personStats.juejin }}</span>
              </div>
              <div class="dist-item">
                <span class="dist-label">巷道</span>
                <n-progress type="line" :percentage="personStats.xiangdaoPercent" :show-indicator="false" />
                <span class="dist-value">{{ personStats.xiangdao }}</span>
              </div>
              <div class="dist-item">
                <span class="dist-label">其他</span>
                <n-progress type="line" :percentage="personStats.otherPercent" :show-indicator="false" />
                <span class="dist-value">{{ personStats.other }}</span>
              </div>
            </div>
          </n-space>
        </n-card>

        <!-- 视频监控 -->
        <n-card title="视频监控" class="panel-card">
          <n-grid :x-gap="8" :y-gap="8" cols="2">
            <n-gi v-for="i in 4" :key="i">
              <div class="video-box">
                <n-icon size="40" color="#666">
                  <VideoCamera />
                </n-icon>
                <span>通道 {{ i }}</span>
              </div>
            </n-gi>
          </n-grid>
        </n-card>

        <!-- 系统状态 -->
        <n-card title="系统状态" class="panel-card">
          <n-space vertical>
            <n-space justify="space-between">
              <span>文件监听</span>
              <n-tag type="success" size="small">正常</n-tag>
            </n-space>
            <n-space justify="space-between">
              <span>数据采集</span>
              <n-tag type="success" size="small">正常</n-tag>
            </n-space>
            <n-space justify="space-between">
              <span>网络连接</span>
              <n-tag type="success" size="small">正常</n-tag>
            </n-space>
          </n-space>
        </n-card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onUnmounted } from 'vue'
import { VideoCamera } from '@vicons/tabler'
import { getCoalMineList } from '@/api/coalmine/coal'

const currentTime = ref('')
let timer: NodeJS.Timeout

// 煤矿列表
const mineList = ref([
  { id: 1, name: '煤矿A', code: '001', online: true },
  { id: 2, name: '煤矿B', code: '002', online: true },
  { id: 3, name: '煤矿C', code: '003', online: false }
])

// 安全监测统计
const safetyStats = reactive({
  total: 156,
  normal: 150,
  alarm: 3,
  fault: 3
})

// 水害监测统计
const waterStats = reactive({
  total: 24,
  percentage: 92
})

// 人员定位统计
const personStats = reactive({
  total: 89,
  caimei: 12,
  caimeiPercent: 13,
  juejin: 8,
  juejinPercent: 9,
  xiangdao: 55,
  xiangdaoPercent: 62,
  other: 14,
  otherPercent: 16
})

// 报警列表
const alarmList = ref([
  { id: 1, sensorName: '甲烷传感器-T1', level: 3, description: '甲烷浓度超限', alarmTime: '14:30:25' },
  { id: 2, sensorName: '一氧化碳传感器', level: 2, description: '一氧化碳浓度预警', alarmTime: '14:28:10' },
  { id: 3, sensorName: '温度传感器', level: 1, description: '温度异常', alarmTime: '14:25:05' }
])

function updateTime() {
  const now = new Date()
  currentTime.value = now.toLocaleString('zh-CN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

function getAlarmType(level: number) {
  const map = { 1: 'default', 2: 'warning', 3: 'error' }
  return map[level] || 'default'
}

function getAlarmLevelName(level: number) {
  const map = { 1: '一般', 2: '重要', 3: '紧急' }
  return map[level] || '未知'
}

function selectMine(mine: any) {
  console.log('选择煤矿', mine)
}

async function loadData() {
  try {
    const res = await getCoalMineList()
    if (res.data && res.data.length > 0) {
      mineList.value = res.data.map((m: any) => ({
        ...m,
        online: true
      }))
    }
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

onMounted(() => {
  updateTime()
  timer = setInterval(updateTime, 1000)
  loadData()
})

onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
  }
})
</script>

<style scoped>
.dashboard {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
  padding: 16px;
  color: #fff;
}

.dashboard-header {
  text-align: center;
  padding: 16px 0;
  position: relative;
}

.dashboard-header h1 {
  font-size: 32px;
  font-weight: bold;
  margin: 0;
  background: linear-gradient(90deg, #00f260, #0575e6);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.header-time {
  position: absolute;
  right: 20px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 18px;
  color: #aaa;
}

.dashboard-content {
  display: flex;
  gap: 16px;
  height: calc(100vh - 100px);
}

.panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.left-panel, .right-panel {
  flex: 0 0 280px;
}

.center-panel {
  flex: 1;
}

.panel-card {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.panel-card :deep(.n-card-header) {
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.panel-card :deep(.n-card-header__main) {
  color: #fff;
}

/* 矿井示意图 */
.mine-map {
  height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mine-visual {
  position: relative;
  width: 100%;
  height: 100%;
  max-width: 400px;
}

.mine-shaft {
  width: 60px;
  height: 120px;
  background: #0575e6;
  border-radius: 4px;
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

.mine-tunnel {
  height: 20px;
  background: rgba(255, 255, 255, 0.2);
  position: absolute;
  top: 130px;
}

.tunnel-1 {
  width: 150px;
  left: 30px;
}

.tunnel-2 {
  width: 150px;
  right: 30px;
}

.mine-workface {
  width: 80px;
  height: 50px;
  background: #f0a020;
  border-radius: 4px;
  position: absolute;
  bottom: 40px;
  left: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
}

.mine-water-pool {
  width: 60px;
  height: 40px;
  background: #18a058;
  border-radius: 4px;
  position: absolute;
  bottom: 40px;
  right: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
}

/* 报警列表 */
.alarm-card :deep(.n-list-item) {
  padding: 8px 0;
}

.alarm-desc {
  color: #d03050;
  margin-right: 8px;
}

.alarm-time {
  color: #888;
  font-size: 12px;
}

/* 人员分布 */
.person-distribution {
  padding: 8px 0;
}

.dist-item {
  display: grid;
  grid-template-columns: 60px 1fr 40px;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}

.dist-label {
  font-size: 12px;
  color: #aaa;
}

.dist-value {
  text-align: right;
  font-size: 14px;
  font-weight: bold;
}

/* 视频框 */
.video-box {
  aspect-ratio: 16/9;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 4px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  font-size: 12px;
  color: #888;
}

/* 滚动条 */
:deep(.n-scrollbar-rail) {
  background: rgba(255, 255, 255, 0.1);
}
</style>
