<template>
  <div>
    <n-card title="历史数据查询" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 搜索栏 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-select
            v-model:value="searchParams.dataType"
            placeholder="数据类型"
            clearable
            :options="dataTypeOptions"
            style="width: 150px"
          />
          <n-input
            v-model:value="searchParams.sensorCode"
            placeholder="传感器编号"
            clearable
            style="width: 150px"
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

        <!-- 数据表格 -->
        <n-card>
          <n-tabs type="line" animated @update:value="changeTab">
            <n-tab-pane name="safety" tab="安全监测历史">
              <n-data-table
                :columns="safetyColumns"
                :data="safetyData"
                :loading="safetyLoading"
                :pagination="{ pageSize: 10 }"
              />
            </n-tab-pane>
            <n-tab-pane name="person" tab="人员定位历史">
              <n-data-table
                :columns="personColumns"
                :data="personData"
                :loading="personLoading"
                :pagination="{ pageSize: 10 }"
              />
            </n-tab-pane>
            <n-tab-pane name="water" tab="水害监测历史">
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
  dataType: 'safety',
  sensorCode: '',
  startTime: '',
  endTime: ''
})

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
const dataTypeOptions = [
  { label: '安全监测', value: 'safety' },
  { label: '人员定位', value: 'person' },
  { label: '水害监测', value: 'water' }
]

// 表格列
const safetyColumns = [
  { title: '时间', key: 'updateTime', width: 160 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '传感器编号', key: 'sensorCode', width: 140 },
  { title: '传感器名称', key: 'sensorName', width: 150 },
  { title: '值', key: 'value', width: 100 },
  { title: '单位', key: 'unit', width: 60 },
  { title: '状态', key: 'status', width: 80 }
]

const personColumns = [
  { title: '时间', key: 'updateTime', width: 160 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '姓名', key: 'personName', width: 80 },
  { title: '卡号', key: 'cardId', width: 120 },
  { title: '区域', key: 'areaName', width: 120 },
  { title: '基站', key: 'stationName', width: 120 }
]

const waterColumns = [
  { title: '时间', key: 'updateTime', width: 160 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '传感器编号', key: 'sensorCode', width: 140 },
  { title: '传感器名称', key: 'sensorName', width: 150 },
  { title: '水位(m)', key: 'waterLevel', width: 100 },
  { title: '流量(m³/h)', key: 'flowRate', width: 100 },
  { title: '状态', key: 'status', width: 80 }
]

async function loadData() {
  // 根据数据类型加载
  if (searchParams.dataType === 'safety') {
    await loadSafetyHistory()
  } else if (searchParams.dataType === 'person') {
    await loadPersonHistory()
  } else if (searchParams.dataType === 'water') {
    await loadWaterHistory()
  }
}

async function loadSafetyHistory() {
  safetyLoading.value = true
  try {
    // 模拟数据
    safetyData.value = [
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', sensorCode: 'CD001', sensorName: '甲烷传感器', value: '0.15', unit: '%', status: '正常' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', sensorCode: 'CD002', sensorName: '一氧化碳传感器', value: '12.5', unit: 'ppm', status: '报警' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿B', sensorCode: 'CD003', sensorName: '温度传感器', value: '28.5', unit: '℃', status: '正常' }
    ]
  } finally {
    safetyLoading.value = false
  }
}

async function loadPersonHistory() {
  personLoading.value = true
  try {
    // 模拟数据
    personData.value = [
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', personName: '张三', cardId: '001', areaName: '采煤工作面', stationName: '基站1' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', personName: '李四', cardId: '002', areaName: '掘进工作面', stationName: '基站2' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿B', personName: '王五', cardId: '003', areaName: '运输巷', stationName: '基站3' }
    ]
  } finally {
    personLoading.value = false
  }
}

async function loadWaterHistory() {
  waterLoading.value = true
  try {
    // 模拟数据
    waterData.value = [
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', sensorCode: 'W001', sensorName: '中央水仓', waterLevel: 2.5, flowRate: 15.8, status: '正常' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿A', sensorCode: 'W002', sensorName: '采区水仓', waterLevel: 1.8, flowRate: 8.2, status: '正常' },
      { updateTime: '2024-01-15 10:00:00', mineName: '煤矿B', sensorCode: 'W003', sensorName: '排水沟', waterLevel: 3.2, flowRate: 25.6, status: '报警' }
    ]
  } finally {
    waterLoading.value = false
  }
}

async function loadMineOptions() {
  try {
    const res = await getCoalMineList()
    mineOptions.value = (res.data || []).map((m: any) => ({ label: m.name, value: m.id }))
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

function changeTab(value: string) {
  searchParams.dataType = value
  loadData()
}

function exportData() {
  message.info('导出功能开发中')
}

onMounted(() => {
  // 设置默认时间范围
  const now = new Date()
  const yesterday = new Date(now.getTime() - 24 * 60 * 60 * 1000)
  searchParams.endTime = now.toISOString().slice(0, 19).replace('T', ' ')
  searchParams.startTime = yesterday.toISOString().slice(0, 19).replace('T', ' ')
  
  loadMineOptions()
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
</style>
