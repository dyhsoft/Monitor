<template>
  <div>
    <n-card title="人员出勤记录" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 统计卡片 -->
        <n-grid :x-gap="16" :y-gap="16" cols="4">
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="今日下井人数" :value="statistics.todayCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="今日出勤率" :value="statistics.attendanceRate + '%'" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="在岗" :value="statistics.onDuty" :value-style="{ color: '#18a058' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="离岗" :value="statistics.offDuty" :value-style="{ color: '#d03050' }" />
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 操作栏 -->
        <n-space>
          <n-button type="primary" @click="loadData">
            <template #icon>
              <n-icon><Refresh /></n-icon>
            </template>
            刷新
          </n-button>
          <n-button @click="exportData">
            <template #icon>
              <n-icon><Download /></n-icon>
            </template>
            导出
          </n-button>
        </n-space>

        <!-- 搜索栏 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-date-picker
            v-model:formatted-value="searchParams.date"
            type="date"
            placeholder="选择日期"
            style="width: 150px"
          />
          <n-select
            v-model:value="searchParams.status"
            placeholder="状态"
            clearable
            :options="statusOptions"
            style="width: 120px"
          />
          <n-button type="primary" @click="loadData">搜索</n-button>
          <n-button @click="handleReset">重置</n-button>
        </n-space>

        <!-- 数据表格 -->
        <n-data-table
          :columns="columns"
          :data="dataList"
          :loading="loading"
          :pagination="pagination"
          :row-key="(row) => row.id"
        />
      </n-space>
    </n-card>

    <!-- 详情弹窗 -->
    <n-modal v-model:show="detailVisible" preset="card" title="出勤详情" style="width: 600px">
      <n-descriptions :column="2" bordered>
        <n-descriptions-item label="姓名">{{ currentRecord.personName }}</n-descriptions-item>
        <n-descriptions-item label="卡号">{{ currentRecord.cardId }}</n-descriptions-item>
        <n-descriptions-item label="部门">{{ currentRecord.department }}</n-descriptions-item>
        <n-descriptions-item label="工种">{{ currentRecord.workType }}</n-descriptions-item>
        <n-descriptions-item label="入井时间">{{ currentRecord.inTime }}</n-descriptions-item>
        <n-descriptions-item label="出井时间">{{ currentRecord.outTime || '未出井' }}</n-descriptions-item>
        <n-descriptions-item label="工作时长">{{ currentRecord.workDuration }}</n-descriptions-item>
        <n-descriptions-item label="状态">
          <n-tag :type="currentRecord.status === 1 ? 'success' : 'default'" size="small">
            {{ currentRecord.status === 1 ? '已出井' : '在井中' }}
          </n-tag>
        </n-descriptions-item>
      </n-descriptions>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Refresh, Download } from '@vicons/tabler'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const dataList = ref([])
const detailVisible = ref(false)
const currentRecord = ref<any>({})

const pagination = reactive({
  page: 1,
  pageSize: 10,
  itemCount: 0,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  showQuickJumper: true
})

const searchParams = reactive({
  mineId: null,
  date: '',
  status: null
})

// 统计
const statistics = reactive({
  todayCount: 0,
  attendanceRate: 0,
  onDuty: 0,
  offDuty: 0
})

// 选项
const mineOptions = ref([])
const statusOptions = [
  { label: '在井中', value: 0 },
  { label: '已出井', value: 1 }
]

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 100 },
  { title: '姓名', key: 'personName', width: 80 },
  { title: '卡号', key: 'cardId', width: 120 },
  { title: '部门', key: 'department', width: 100 },
  { title: '入井时间', key: 'inTime', width: 150 },
  { title: '出井时间', key: 'outTime', width: 150 },
  { title: '工作时长', key: 'workDuration', width: 100 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => row.status === 1 ? '已出井' : '在井中' },
  {
    title: '操作',
    key: 'actions',
    width: 80,
    render: (row: any) => {
      return h(NButton, { size: 'small', onClick: () => showDetail(row) }, () => '详情')
    }
  }
]

function showDetail(row: any) {
  currentRecord.value = row
  detailVisible.value = true
}

async function loadData() {
  loading.value = true
  try {
    // 模拟数据
    dataList.value = [
      { id: 1, mineName: '煤矿A', personName: '张三', cardId: '001', department: '采煤队', inTime: '2024-01-15 08:00:00', outTime: '2024-01-15 18:30:00', workDuration: '10小时30分', status: 1 },
      { id: 2, mineName: '煤矿A', personName: '李四', cardId: '002', department: '掘进队', inTime: '2024-01-15 08:30:00', outTime: '', workDuration: '', status: 0 },
      { id: 3, mineName: '煤矿B', personName: '王五', cardId: '003', department: '通风队', inTime: '2024-01-15 07:00:00', outTime: '2024-01-15 19:00:00', workDuration: '12小时', status: 1 },
      { id: 4, mineName: '煤矿B', personName: '赵六', cardId: '004', department: '机电队', inTime: '2024-01-15 09:00:00', outTime: '2024-01-15 17:00:00', workDuration: '8小时', status: 1 }
    ]
    pagination.itemCount = dataList.value.length
    
    // 统计
    statistics.todayCount = 45
    statistics.attendanceRate = 92
    statistics.onDuty = dataList.value.filter((r: any) => r.status === 0).length
    statistics.offDuty = dataList.value.filter((r: any) => r.status === 1).length
  } finally {
    loading.value = false
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

function handleReset() {
  searchParams.mineId = null
  searchParams.date = ''
  searchParams.status = null
  loadData()
}

function exportData() {
  message.info('导出功能开发中')
}

onMounted(() => {
  // 设置默认日期
  searchParams.date = new Date().toISOString().slice(0, 10)
  loadMineOptions()
  loadData()
})
</script>

<script lang="ts">
import { h } from 'vue'
import { NButton } from 'naive-ui'

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
