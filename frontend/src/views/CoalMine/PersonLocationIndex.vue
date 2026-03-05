<template>
  <div class="person-location">
    <!-- 统计 -->
    <a-row :gutter="16" class="mb-4">
      <a-col :span="6">
        <a-card>
          <a-statistic title="井下总人数" :value="statistics.totalCount" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="采煤面人数" :value="statistics.caimeiCount" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="掘进面人数" :value="statistics.juejinCount" />
        </a-card>
      </a-col>
      <a-col :span="6">
        <a-card>
          <a-statistic title="巷道人数" :value="statistics.xiangdaoCount" />
        </a-card>
      </a-col>
    </a-row>

    <!-- 搜索 -->
    <a-card class="mb-4">
      <a-form layout="inline" :model="searchForm">
        <a-form-item label="煤矿">
          <a-select v-model:value="searchForm.mineId" placeholder="请选择煤矿" style="width: 200px" allow-clear>
            <a-select-option v-for="item in mineList" :key="item.id" :value="item.id">
              {{ item.name }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item label="姓名">
          <a-input v-model:value="searchForm.personName" placeholder="姓名" style="width: 120px" />
        </a-form-item>
        <a-form-item label="卡号">
          <a-input v-model:value="searchForm.cardId" placeholder="定位卡号" style="width: 150px" />
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

    <!-- 表格 -->
    <a-card>
      <BasicTable :tableConfig="tableConfig" :data="dataList" :loading="loading" :pagination="pagination">
        <template #columns>
          <a-table-column title="姓名" data-index="personName" key="personName" width="100" />
          <a-table-column title="卡号" data-index="cardId" key="cardId" width="120" />
          <a-table-column title="煤矿" data-index="mineName" key="mineName" width="120" />
          <a-table-column title="所在区域" data-index="areaName" key="areaName" />
          <a-table-column title="基站" data-index="stationName" key="stationName" width="150" />
          <a-table-column title="坐标" key="position" width="150">
            <template #cell="{ record }">
              X: {{ record.x }} Y: {{ record.y }} Z: {{ record.z }}
            </template>
          </a-table-column>
          <a-table-column title="进入时间" data-index="inTime" key="inTime" width="160" />
          <a-table-column title="更新时间" data-index="updateTime" key="updateTime" width="160" />
          <a-table-column title="操作" key="action" width="100">
            <template #cell="{ record }">
              <a-button type="link" size="small" @click="handleTrack(record)">轨迹</a-button>
            </a-template>
          </a-table-column>
        </template>
      </BasicTable>
    </a-card>

    <!-- 轨迹弹窗 -->
    <a-modal
      v-model:open="trackVisible"
      title="人员轨迹"
      width="900px"
      :footer="null"
    >
      <a-timeline>
        <a-timeline-item v-for="item in trackList" :key="item.stationId" :color="item.color">
          <p>{{ item.areaName }}</p>
          <p>{{ item.stationName }}</p>
          <p>进入: {{ item.inTime }}</p>
          <p v-if="item.outTime">离开: {{ item.outTime }}</p>
          <p>停留: {{ item.duration }}秒</p>
        </a-timeline-item>
      </a-timeline>
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
const mineList = ref([]);
const trackVisible = ref(false);
const trackList = ref([]);

const statistics = reactive({
  totalCount: 0,
  caimeiCount: 0,
  juejinCount: 0,
  xiangdaoCount: 0
});

const searchForm = reactive({
  mineId: null,
  personName: '',
  cardId: ''
});

const pagination = reactive({
  current: 1,
  pageSize: 20,
  total: 0
});

const tableConfig = reactive({
  api: '/api/CoalMine/Person/realtime-page',
  method: 'POST',
  columns: [],
  pagination: true,
  showIndex: true
});

// 加载数据
const loadData = async () => {
  loading.value = true;
  try {
    // TODO: 调用API
  } finally {
    loading.value = false;
  }
};

// 重置
const handleReset = () => {
  searchForm.mineId = null;
  searchForm.personName = '';
  searchForm.cardId = '';
  loadData();
};

// 轨迹
const handleTrack = (record) => {
  trackVisible.value = true;
  // TODO: 加载轨迹数据
};

onMounted(() => {
  loadData();
});
</script>

<style scoped>
.mb-4 { margin-bottom: 16px; }
</style>
