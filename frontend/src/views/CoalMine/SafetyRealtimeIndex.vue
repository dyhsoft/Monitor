<template>
  <div class="safety-monitoring">
    <!-- 统计卡片 -->
    <a-row :gutter="16" class="mb-4">
      <a-col :span="6">
        <a-card>
          <a-statistic title="传感器总数" :value="statistics.totalCount" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="正常" :value="statistics.normalCount" :value-style="{ color: '#52c41a' }" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="报警" :value="statistics.alarmCount" :value-style="{ color: '#ff4d4f' }" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="断电" :value="statistics.powerOffCount" :value-style="{ color: '#faad14' }" />
        </a-card>
      </a-col>
    </a-row>

    <!-- 报警列表 -->
    <a-alert
      v-if="alarmList.length > 0"
      message="当前存在报警数据"
      type="error"
      class="mb-4"
      closable
    >
      <template #description>
        <a-list size="small" :data-source="alarmList">
          <template #renderItem="{ item }">
            <a-list-item>
              <a-list-item-meta>
                <template #title>
                  <span style="color: #ff4d4f">{{ item.sensorCode }}</span>
                </template>
                <template #description>
                  {{ item.sensorName }} - 当前值: {{ item.value }} {{ item.unit }}
                </template>
              </a-list-item-meta>
            </a-list-item>
          </template>
        </a-list>
      </template>
    </a-alert>

    <!-- 搜索条件 -->
    <a-card class="mb-4">
      <a-form layout="inline" :model="searchForm">
        <a-form-item label="煤矿">
          <a-select v-model:value="searchForm.mineId" placeholder="请选择煤矿" style="width: 200px" allow-clear>
            <a-select-option v-for="item in mineList" :key="item.id" :value="item.id">
              {{ item.name }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="测点编号">
          <a-input v-model:value="searchForm.sensorCode" placeholder="测点编号" style="width: 150px" />
        </a-form-item>
        <a-form-item label="状态">
          <a-select v-model:value="searchForm.status" placeholder="请选择" style="width: 120px" allow-clear>
            <a-select-option :value="0">正常</a-select-option>
            <a-select-option :value="1">报警</a-select-option>
            <a-select-option :value="2">断电</a-select-option>
            <a-select-option :value="3">故障</a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item>
          <a-space>
            <a-button type="primary" @click="loadData">
              <template #icon><SearchOutlined /></template>
              查询
            </a-button>
            <a-button @click="handleReset">
              <template #icon><ReloadOutlined /></template>
              重置
            </a-button>
          </a-space>
        </a-form-item>
      </a-form>
    </a-card>

    <!-- 数据表格 -->
    <a-card>
      <BasicTable :tableConfig="tableConfig" :data="dataList" :loading="loading" :pagination="pagination">
        <template #columns>
          <a-table-column title="测点编号" data-index="sensorCode" key="sensorCode" width="180" />
          <a-table-column title="测点名称" data-index="sensorName" key="sensorName" ellipsis />
          <a-table-column title="煤矿" data-index="mineName" key="mineName" width="120" />
          <a-table-column title="监测值" data-index="value" key="value" width="100" align="right">
            <template #cell="{ record }">
              <span :style="{ color: getValueColor(record.status) }">
                {{ record.value }} {{ record.unit }}
              </span>
            </template>
          </a-table-column>
          <a-table-column title="状态" data-index="statusName" key="statusName" width="80">
            <template #cell="{ record }">
              <a-tag :color="getStatusColor(record.status)">
                {{ record.statusName }}
              </a-tag>
            </template>
          </a-table-column>
          <a-table-column title="更新时间" data-index="updateTime" key="updateTime" width="160" />
          <a-table-column title="操作" key="action" width="120">
            <template #cell="{ record }">
              <a-space>
                <a-button type="link" size="small" @click="handleTrend(record)">趋势</a-button>
                <a-button type="link" size="small" @click="handleDetail(record)">详情</a-button>
              </a-space>
            </template>
          </a-table-column>
        </template>
      </BasicTable>
    </a-card>

    <!-- 趋势图弹窗 -->
    <a-modal
      v-model:open="trendVisible"
      title="数据趋势"
      width="900px"
      :footer="null"
    >
      <div id="trendChart" style="height: 400px"></div>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import { BasicTable } from '@/components/Table';
import { SearchOutlined, ReloadOutlined } from '@ant-design/icons-vue';

const loading = ref(false);
const dataList = ref([]);
const alarmList = ref([]);
const mineList = ref([]);
const trendVisible = ref(false);

const statistics = reactive({
  totalCount: 0,
  normalCount: 0,
  alarmCount: 0,
  powerOffCount: 0
});

const searchForm = reactive({
  mineId: null,
  sensorCode: '',
  status: null
});

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0,
  showSizeChanger: true,
  showTotal: (total) => `共 ${total} 条`
});

const tableConfig = reactive({
  api: '/api/CoalMine/Safety/realtime-page',
  method: 'POST',
  columns: [],
  pagination: true,
  showIndex: true
});

 = (status)const getStatusColor => {
  const colorMap = {
    0: 'green',
    1: 'red',
    2: 'orange',
    3: 'default',
    4: 'gray'
  };
  return colorMap[status] || 'default';
};

const getValueColor = (status) => {
  if (status === 1) return '#ff4d4f';
  if (status === 2) return '#faad14';
  return '';
};

// 加载数据
const loadData = async () => {
  loading.value = true;
  try {
    // TODO: 调用API
  } finally {
    loading.value = false;
  }
};

// 加载统计
const loadStatistics = async () => {
  // TODO: 调用API
};

// 加载报警列表
const loadAlarmList = async () => {
  // TODO: 调用API
};

// 重置
const handleReset = () => {
  searchForm.mineId = null;
  searchForm.sensorCode = '';
  searchForm.status = null;
  loadData();
};

// 趋势
const handleTrend = (record) => {
  trendVisible.value = true;
  // TODO: 加载趋势数据并渲染图表
};

// 详情
const handleDetail = (record) => {
  // TODO: 显示详情
};

onMounted(() => {
  loadData();
  loadStatistics();
  loadAlarmList();
});
</script>

<style scoped>
.mb-4 {
  margin-bottom: 16px;
}
</style>
