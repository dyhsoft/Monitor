<template>
  <div>
    <n-card title="数据统计分析" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 时间筛选 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-date-picker
            v-model:formatted-value="searchParams.startTime"
            type="datetime"
            placeholder="开始时间"
            style="width: 200px"
          />
          <n-date-picker
            v-model:formatted-value="searchParams.endTime"
            type="datetime"
            placeholder="结束时间"
            style="width: 200px"
          />
          <n-button type="primary" @click="loadData">查询</n-button>
          <n-button @click="exportData">
            <template #icon>
              <n-icon><Download /></n-icon>
            </template>
            导出
          </n-button>
        </n-space>

        <!-- 统计卡片 -->
        <n-grid :x-gap="16" :y-gap="16" cols="4">
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="安全测点在线" :value="statistics.safetyOnline" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="安全测点报警" :value="statistics.safetyAlarm" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="井下人员总数" :value="statistics.personCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="水害监测点" :value="statistics.waterCount" />
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 图表区域 -->
        <n-grid :x-gap="16" :y-gap="16" cols="2">
          <!-- 安全监测趋势图 -->
          <n-gi>
            <n-card title="安全监测报警趋势">
              <div ref="safetyChartRef" style="height: 300px"></div>
            </n-card>
          </n-gi>
          <!-- 人员分布饼图 -->
          <n-gi>
            <n-card title="人员区域分布">
              <div ref="personChartRef" style="height: 300px"></div>
            </n-card>
          </n-gi>
          <!-- 水害监测趋势图 -->
          <n-gi>
            <n-card title="水害监测趋势">
              <div ref="waterChartRef" style="height: 300px"></div>
            </n-card>
          </n-gi>
          <!-- 报警类型分布 -->
          <n-gi>
            <n-card title="报警类型分布">
              <div ref="alarmTypeChartRef" style="height: 300px"></div>
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 数据表格 -->
        <n-card title="详细数据">
          <n-tabs type="line" animated>
            <n-tab-pane name="safety" tab="安全监测">
              <n-data-table
                :columns="safetyColumns"
                :data="safetyData"
                :loading="safetyLoading"
                :pagination="{ pageSize: 10 }"
              />
            </n-tab-pane>
            <n-tab-pane name="person" tab="人员统计">
              <n-data-table
                :columns="personColumns"
                :data="personData"
                :loading="personLoading"
                :pagination="{ pageSize: 10 }"
              />
            </n-tab-pane>
            <n-tab-pane name="water" tab="水害监测">
              <n-data-table
                :columns="waterColumns"
                :data="waterData"
                :loading="waterLoading"
                :pagination="{ pageSize: 10 }"
              />
            </n-tab-pane>
          </n-tabs>
        </n-card>
      </n-space>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Download } from '@vicons/tabler'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

// 搜索参数
const searchParams = reactive({
  mineId: null,
  startTime: '',
  endTime: ''
})

// 统计
const statistics = reactive({
  safetyOnline: 0,
  safetyAlarm: 0,
  personCount: 0,
  waterCount: 0
})

// 图表引用
const safetyChartRef = ref()
const personChartRef = ref()
const waterChartRef = ref()
const alarmTypeChartRef = ref()

// 加载状态
const safetyLoading = ref(false)
const personLoading = ref(false)
const waterLoading = ref(false)

// 数据
const safetyData = ref([])
const personData = ref([])
const waterData = ref([])

// 选项
const mineOptions = ref([])

// 表格列
const safetyColumns = [
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '测点编号', key: 'sensorCode', width: 140 },
  { title: '测点名称', key: 'sensorName', width: 150 },
  { title: '当前值', key: 'value', width: 100 },
  { title: '单位', key: 'unit', width: 60 },
  { title: '状态', key: 'status', width: 80 }
]

const personColumns = [
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '区域', key: 'areaName', width: 150 },
  { title: '人数', key: 'personCount', width: 100 },
  { title: '进入时间', key: 'inTime', width: 160 }
]

const waterColumns = [
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '传感器编号', key: 'sensorCode', width: 140 },
  { title: '传感器名称', key: 'sensorName', width: 150 },
  { title: '水位(m)', key: 'waterLevel', width: 100 },
  { title: '流量(m³/h)', key: 'flowRate', width: 100 },
  { title: '状态', key: 'status', width: 80 }
]

async function loadData() {
  // 模拟数据加载
  statistics.safetyOnline = 156
  statistics.safetyAlarm = 3
  statistics.personCount = 89
  statistics.waterCount = 24
  
  // 模拟详细数据
  safetyData.value = [
    { mineName: '煤矿A', sensorCode: 'CD001', sensorName: '甲烷传感器', value: '0.12', unit: '%', status: '正常' },
    { mineName: '煤矿A', sensorCode: 'CD002', sensorName: '一氧化碳传感器', value: '8.5', unit: 'ppm', status: '正常' },
    { mineName: '煤矿B', sensorCode: 'CD003', sensorName: '温度传感器', value: '28.5', unit: '℃', status: '报警' }
  ]
  
  personData.value = [
    { mineName: '煤矿A', areaName: '采煤工作面', personCount: 12, inTime: '2024-01-15 08:00' },
    { mineName: '煤矿A', areaName: '掘进工作面', personCount: 8, inTime: '2024-01-15 08:30' },
    { mineName: '煤矿A', areaName: '巷道', personCount: 25, inTime: '2024-01-15 07:00' }
  ]
  
  waterData.value = [
    { mineName: '煤矿A', sensorCode: 'W001', sensorName: '中央水仓', waterLevel: 2.5, flowRate: 15.8, status: '正常' },
    { mineName: '煤矿A', sensorCode: 'W002', sensorName: '采区水仓', waterLevel: 1.8, flowRate: 8.2, status: '正常' },
    { mineName: '煤矿B', sensorCode: 'W003', sensorName: '排水沟', waterLevel: 3.2, flowRate: 25.6, status: '报警' }
  ]
  
  // 初始化图表
  initCharts()
}

function initCharts() {
  // 安全监测趋势图 - 使用模拟数据
  if (safetyChartRef.value) {
    // 实际项目中可使用 echarts
    safetyChartRef.value.innerHTML = '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:#999;">安全监测报警趋势图表</div>'
  }
  
  // 人员分布饼图
  if (personChartRef.value) {
    personChartRef.value.innerHTML = '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:#999;">人员区域分布图表</div>'
  }
  
  // 水害监测趋势图
  if (waterChartRef.value) {
    waterChartRef.value.innerHTML = '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:#999;">水害监测趋势图表</div>'
  }
  
  // 报警类型分布
  if (alarmTypeChartRef.value) {
    alarmTypeChartRef.value.innerHTML = '<div style="display:flex;align-items:center;justify-content:center;height:100%;color:#999;">报警类型分布图表</div>'
  }
}

function exportData() {
  message.info('导出功能开发中')
}

async function loadMineOptions() {
  try {
    const res = await getCoalMineList()
    mineOptions.value = (res.data || []).map((m: any) => ({ label: m.name, value: m.id }))
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

onMounted(() => {
  loadMineOptions()
  // 设置默认时间范围
  const now = new Date()
  const yesterday = new Date(now.getTime() - 24 * 60 * 60 * 1000)
  searchParams.endTime = now.toISOString().slice(0, 19).replace('T', ' ')
  searchParams.startTime = yesterday.toISOString().slice(0, 19).replace('T', ' ')
  loadData()
})
</script>

<script lang="ts">
export default {
  setup() {
    return {}
  }
}
</script>

<style scoped>
.proCard {
  background: #fff;
}

.stat-card {
  text-align: center;
}
</style>
